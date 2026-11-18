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
}