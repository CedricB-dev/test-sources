using BlazorTDD.App.Components.Pages;
using Bunit;
using FluentAssertions;

namespace BlazorTDD.Tests;

public class UnitTest1
{
    [Fact]
    public void ShouldIncrementCountWhenClickingButton()
    {
        using var testContext = new TestContext();

        var component = testContext.RenderComponent<Counter>();
        var button = component.Find("button");
        button.Click();

        var currentCount = component.Find("[role='status']");
        currentCount.TextContent.Should().Be("Current count: 1");
    }
    
    [Fact]
    public void ShouldAbc()
    {
        using var testContext = new TestContext();
        
        var component = testContext.RenderComponent<Weather>();
        component.WaitForElement(".table", TimeSpan.FromSeconds(4));

        var elements = component.FindAll("tbody > tr");
        elements.Count().Should().Be(5);
    }
}