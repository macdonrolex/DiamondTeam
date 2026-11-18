using System.Threading;
using ContosoCrafts.WebSite.Components;
using NUnit.Framework;

namespace UnitTests.Components;

/// <summary>
/// Unit tests for Notification.razor
/// </summary>
public class NotificationTests: BunitTestContext
{
    #region OnInitialized
    
    /// <summary>
    /// The notification with empty message should not been displayed.
    /// </summary>
    [Test]
    public void OnInitialized_InValid_Message_Empty_Should_Make_IsVisible_False()
    {

        // Arrange
        
        // Not needed

        // Act
        
        // Render the notification
        var page = Render<Notification>();
        
        // Markup for notification for assertion
        var markup = page.Markup;
        
        // Reset

        // Not needed

        // Assert
        
        // The IsVisible is set to false
        Assert.That(page.Instance.IsVisible, Is.False);
        
        // No notification showed up
        Assert.That(markup, Is.Empty);
    }
    
    /// <summary>
    /// The notification with a message should have been displayed.
    /// </summary>
    [Test]
    public void OnInitialized_Valid_Has_Message_Should_Display_Notification()
    {

        // Arrange
        
        // Not needed
        
        // Act
        
        // Render the notification with a message
        var page = Render<Notification>(parameters => 
            parameters.Add(p => p.Message, "test_message"));
        
        // Markup for notification for assertion
        var markup = page.Markup;
        
        // Reset

        // Not needed

        // Assert
        
        // The IsVisible is true
        Assert.That(page.Instance.IsVisible, Is.True);
        
        // Display the notification we want
        Assert.That(markup.Contains("test_message"), Is.True);
    }
    
    #endregion OnInitialized

    #region OnAfterRenderAsync
    
    /// <summary>
    /// Notification should be closed after the given display milliseconds.
    /// </summary>
    [Test]
    public void OnAfterRenderAsync_After_Waiting_Should_Close_Notification()
    {

        // Arrange
        
        // Render the notification with a message, and make it last 0.5 second
        var page = Render<Notification>(parameters => 
            parameters.Add(p => p.Message, "test_message")
                      .Add(p => p.DisplayMilliseconds, 500));
        
        // Markup for notification before it closes
        var preMarkup = page.Markup;
        
        // Act
        
        // Wait for 1 second for the notification to disappear
        Thread.Sleep(1000);
        
        // Markup for notification after it closed
        var postMarkup = page.Markup;
        
        // Reset

        // Not needed

        // Assert
        
        // The IsVisible is true because it should have been closed after 0.5 second
        Assert.That(page.Instance.IsVisible, Is.False);
        
        // Display the notification we want before it closes
        Assert.That(preMarkup.Contains("test_message"), Is.True);
        
        // Display nothing after it closed
        Assert.That(postMarkup, Is.Empty);
    }

    #endregion OnAfterRenderAsync
}