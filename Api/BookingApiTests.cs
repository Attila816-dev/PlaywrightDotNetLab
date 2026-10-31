using System.Text.Json;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightDotNetLab.Api;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class BookingApiTests : ApiTestBase
{
    private static object NewBooking(string firstName) => new
    {
        firstname = firstName,
        lastname = "Playwright",
        totalprice = 150,
        depositpaid = true,
        bookingdates = new { checkin = "2026-12-01", checkout = "2026-12-05" },
        additionalneeds = "Breakfast"
    };

    private async Task<JsonElement> JsonOf(IAPIResponse response)
        => (await response.JsonAsync())!.Value;

    [Test]
    public async Task CreateBooking_WithValidPayload_ReturnsIdAndEchoesData()
    {
        var name = $"Anna-{Guid.NewGuid():N}";

        var response = await Api.PostAsync("/booking", new() { DataObject = NewBooking(name) });

        await Expect(response).ToBeOKAsync();
        var json = await JsonOf(response);
        Assert.Multiple(() =>
        {
            Assert.That(json.GetProperty("bookingid").GetInt32(), Is.GreaterThan(0));
            Assert.That(json.GetProperty("booking").GetProperty("firstname").GetString(), Is.EqualTo(name));
            Assert.That(json.GetProperty("booking").GetProperty("totalprice").GetInt32(), Is.EqualTo(150));
        });
    }

    [Test]
    public async Task GetBooking_AfterCreate_ReturnsSameBooking()
    {
        var name = $"Ben-{Guid.NewGuid():N}";
        var created = await JsonOf(await Api.PostAsync("/booking", new() { DataObject = NewBooking(name) }));
        var id = created.GetProperty("bookingid").GetInt32();

        var response = await Api.GetAsync($"/booking/{id}");

        await Expect(response).ToBeOKAsync();
        var booking = await JsonOf(response);
        Assert.That(booking.GetProperty("firstname").GetString(), Is.EqualTo(name));
        Assert.That(booking.GetProperty("bookingdates").GetProperty("checkin").GetString(), Is.EqualTo("2026-12-01"));
    }

    [Test]
    public async Task UpdateThenDeleteBooking_WithToken_RemovesIt()
    {
        // Arrange: token + a booking
        var auth = await Api.PostAsync("/auth", new() { DataObject = new { username = "admin", password = "password123" } });
        await Expect(auth).ToBeOKAsync();

        var token = (await JsonOf(auth)).GetProperty("token").GetString()!;
        var authHeaders = new Dictionary<string, string> { ["Cookie"] = $"token={token}" };

        var created = await JsonOf(await Api.PostAsync("/booking", new() { DataObject = NewBooking("Cara") }));
        var id = created.GetProperty("bookingid").GetInt32();

        // Act + assert: update
        var update = await Api.PutAsync($"/booking/{id}", new() { DataObject = NewBooking("Cara-Updated"), Headers = authHeaders });
        await Expect(update).ToBeOKAsync();
        Assert.That((await JsonOf(update)).GetProperty("firstname").GetString(), Is.EqualTo("Cara-Updated"));

        // Act + assert: delete (Restful-Booker answers 201 Created, a known quirk)
        var delete = await Api.DeleteAsync($"/booking/{id}", new() { Headers = authHeaders });
        Assert.That(delete.Status, Is.EqualTo(201));

        var afterDelete = await Api.GetAsync($"/booking/{id}");
        Assert.That(afterDelete.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task GetBooking_WithUnknownId_Returns404()
    {
        var response = await Api.GetAsync("/booking/0");

        Assert.That(response.Status, Is.EqualTo(404));
    }
}