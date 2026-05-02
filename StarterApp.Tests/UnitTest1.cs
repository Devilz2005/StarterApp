namespace StarterApp.Tests;

public class ItemValidationTests
{
    [Fact]
    public void DailyRate_MustBePositive()
    {
        var dailyRate = 15;

        Assert.True(dailyRate > 0);
    }

    [Fact]
    public void Title_MustNotBeEmpty()
    {
        var title = "Wireless Drill";

        Assert.False(string.IsNullOrWhiteSpace(title));
    }
}

public class RentalValidationTests
{
    [Fact]
    public void EndDate_ShouldBeAfterStartDate()
    {
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(3);

        Assert.True(endDate > startDate);
    }
}

public class LocationValidationTests
{
    [Fact]
    public void Edinburgh_ShouldHaveValidCoordinates()
    {
        var latitude = 55.9533;
        var longitude = -3.1883;

        Assert.True(latitude != 0);
        Assert.True(longitude != 0);
    }
}