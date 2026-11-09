using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Bunit;
using NUnit.Framework;

using ContosoCrafts.WebSite.Components;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;

namespace UnitTests.Components
{
    /// <summary>
    /// Unit tests for ProductList.razor
    /// </summary>
    public class ProductListTests : BunitTestContext
    {
        #region TestSetup

        [SetUp]
        public void TestInitialize()
        {
        }
        #endregion TestSetup
        /// <summary>
        /// Test whether the home page can retrieve data from database
        /// </summary>
        [Test]
        public void ProductList_Valid_Default_Should_Return_Content()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);

            // Act
            var page = Render<ProductList>();

            // Get the Cards retrned
            var result = page.Markup;
            
            // Reset
            // Not needed

            // Assert
            Assert.That(result.Contains("Minecraft"), Is.True);
        }

        #region SelectProduct
        /// <summary>
        ///  Open the popup, and see if the content is what we want.
        /// </summary>
        [Test]
        public void SelectProduct_Valid_ID_jenlooper_Should_Return_Content()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var id = "MoreInfoButton_jenlooper-cactus";

            var page = Render<ProductList>();
            
            // Find the Buttons (more info)
            var buttonList = page.FindAll<ProductList>("Button");
            
            // Find the one that matches the ID looking for and click it
            var button = buttonList.First((m => m.OuterHtml.Contains(id)));
            
            // Act
            button.Click();
            
            // Get the markup to use for the assert
            var pageMarkup = page.Markup;

            // Reset
            // Not Needed

            // Assert
            Assert.That(
                pageMarkup.Contains(
                    "Minecraft is a sandbox video game developed and published by the Swedish company Mojang Studios."),
                Is.True);
        }
        #endregion SelectProduct

        #region SubmitRating
        /// <summary>
        /// This test tests that the SubmitRating will change the vote as well as the Star checked
        /// Because the star check is a calculation of the ratings, using a record that has no stars and checking one
        /// makes it clear what was changed.
        /// 
        /// The test needs to open the page
        /// Then open the popup on the card
        /// Then record the state of the count and star check status
        /// Then check a star
        /// Then check again the state of the cound and star check status
        /// </summary>
        [Test]
        public void SubmitRating_Valid_ID_Click_Unstared_Should_Increment_Count_And_Check_Star()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var id = "MoreInfoButton_jenlooper-light";
            var page = Render<ProductList>();
            // Find the Buttons (more info)
            var buttonList = page.FindAll("Button");
            // Find the one that matches the ID looking for and click it
            var button = buttonList.First((m => m.OuterHtml.Contains(id)));
            button.Click();
            
            // Get the markup of the page post the Click action
            var buttonMarkup = page.Markup;
            
            // Get the star buttons
            var preStarButtonList = page.FindAll("span");
            
            // Get the Vote Count, the sixth span in the List is the first star 
            var preVoteCountSpan = preStarButtonList[5];
            var preVoteCountString = preVoteCountSpan.OuterHtml;
            
            // Get the First star item from the List, it should not be checked
            var starButton = preStarButtonList.First(m => !string.IsNullOrEmpty(m.ClassName)
                                                       && m.ClassName.Contains("fa fa-star"));
            
            // Save the html for it to compare after the click
            var preStarChange = starButton.OuterHtml;
            
            // Backup original database for resetting.
            var backupData = TestHelper.ProductService.GetAllData().FirstOrDefault(x => x.Id == "jenlooper-light");
                
            // Act
            
            // Click the star button
            starButton.Click();
            
            // Get the markup to use for teh assert
            buttonMarkup = page.Markup;
            
            // Get the star buttons after clicking
            var postStarButtonList = page.FindAll("span");
            
            // Get the Vote Count
            var postVoteCountSpan = postStarButtonList[5];
            var postVoteCountString = postVoteCountSpan.OuterHtml;
            
            // Get the Last stared item from the list
            starButton = postStarButtonList.First(m => !string.IsNullOrEmpty(m.ClassName)
                                                   && m.ClassName.Contains("fa fa-star checked"));
            
            // Save the html for it to compare after the click
            var postStarChange = starButton.OuterHtml;
            
            // Reset
            // Update with the original data in database
            TestHelper.ProductService.UpdateData(backupData);
            
            // Assert
            Assert.That(preVoteCountString.Contains("Be the first to vote!"), Is.True);
            Assert.That(postVoteCountString.Contains("1 Vote"), Is.True);
            Assert.That(preVoteCountString.Equals(postVoteCountString), Is.False);
            Assert.That(preStarChange.Equals(postStarChange), Is.False);
        }
        #endregion SubmitRating
        
        #region ShowNewCommentInput
        /// <summary>
        /// Click the Add Comment button should display the input field and the Save Comment button
        /// </summary>
        [Test]
        public void ShowNewCommentInput_Valid_Click_AddComment_Should_Display_Comment_Edit()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var id = "MoreInfoButton_jenlooper-cactus";
            var page = Render<ProductList>();
            // Find the Buttons (more info)
            var buttonList = page.FindAll("Button");
            // Find the one that matches the ID looking for and click it
            var button = buttonList.First((m => m.OuterHtml.Contains(id)));
            button.Click();
            
            // Get the Add Comment button
            id = "AddComment";
            buttonList = page.FindAll("Button");
            var addCommentButton = buttonList.First((m => m.OuterHtml.Contains(id)));
            
            // Get the markup for the assertion
            var preButtonMarkup = page.Markup;
                
            // Act
            // Click the Add Comment button
            addCommentButton.Click();
            
            // Get the post click markup 
            var postButtonMarkup = page.Markup;
            
            // Reset
            
            // Assert
            // Before clicking, no input field and Save Comment button
            Assert.That(preButtonMarkup.Contains("input type=\"text\""), Is.False);
            Assert.That(preButtonMarkup.Contains("Save Comment"), Is.False);
            // After clicking, has input field and Save Comment button
            Assert.That(postButtonMarkup.Contains("input type=\"text\""), Is.True);
            Assert.That(postButtonMarkup.Contains("Save Comment"), Is.True);
        }
        #endregion ShowNewCommentInput
        
        #region UpdateCommentText
        
        /// <summary>
        /// Test whether we can type in the comment, save it, and display it
        /// </summary>
        [Test]
        public void UpdateCommentText_Valid_TypeIn_Text_And_Save_Should_Display_The_Comment()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var id = "MoreInfoButton_jenlooper-cactus";
            var page = Render<ProductList>();
            // Find the Buttons (more info)
            var buttonList = page.FindAll("Button");
            // Find the one that matches the ID looking for and click it
            var button = buttonList.First((m => m.OuterHtml.Contains(id)));
            button.Click();
            
            // Get the Add Comment button
            id = "AddComment";
            buttonList = page.FindAll("Button");
            var addCommentButton = buttonList.First((m => m.OuterHtml.Contains(id)));
            
            // Get the markup for the assertion
            var preButtonMarkup = page.Markup;
            
            // Backup original database for resetting.
            var backupData = TestHelper.ProductService.GetAllData().FirstOrDefault(x => x.Id == "jenlooper-cactus");
                
            // Act
            // Click the Add Comment button
            addCommentButton.Click();
            
            // Type in some text
            page.Find("input[type='text']").Change("test_comment");
            
            // Get the save button and click it
            buttonList = page.FindAll("Button");
            id = "SaveComment";
            var saveButton = buttonList.First((m => m.OuterHtml.Contains(id)));
            saveButton.Click();
            
            // Get the post click markup 
            var postButtonMarkup = page.Markup;
            
            // Reset
            // Update with the original data in database
            TestHelper.ProductService.UpdateData(backupData);
            
            // Assert
            // Before clicking, no comment
            Assert.That(preButtonMarkup.Contains("test_comment"), Is.False);
            // After clicking, show the comment
            Assert.That(postButtonMarkup.Contains("test_comment"), Is.True);
        }
        #endregion UpdateCommentText

        #region ContainsSearchText
        
        /// <summary>
        /// Test whether the search function cna handle the situation that a post contains no content
        /// or only white spaces
        /// </summary>
        [Test]
        public void ContainsSearchText_InValid_TargetText_Null_Or_WhiteSpace_Should_Return_False()
        {
            // Arrange
            
            // The white spaces target text
            var dataText = "  ";
            
            // The query provided by user
            var dataQuery = "query";

            // Act
            var result = ProductList.ContainsSearchText(dataText, dataQuery);

            // Reset
            // Not needed

            // Assert
            Assert.That(result, Is.EqualTo(false));
        }
        
        /// <summary>
        /// Test typing in unmatched text should display "No matching posts found."
        /// </summary>
        [Test]
        public void ContainsSearchText_InValid_TypeIn_Unmatched_Text_Should_Display_No_Matching()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            
            // Backup original database for resetting.
            var backupData = TestHelper.ProductService.GetAllData();
            
            // Delete all but first data in Database
            foreach (var e in backupData.Skip(1))
            {
                TestHelper.ProductService.DeleteData(e.Id);
            }
            
            // Make a fake post into database and try to search it
            var post = TestHelper.ProductService.GetAllData().First();
            post.Title = "test_title";
            post.Maker = "teest_maker";
            post.Description = "test_description";
            TestHelper.ProductService.UpdateData(post);
            
            // Render the page
            var page = Render<ProductList>();
            
            // Get the markup for the assertion
            var preMarkup = page.Markup;
            
            // Act
            // Type unmatched text into the search box
            page.Find("input[id=\"post-search\"]").Input("unmatched_text");
            
            // Get the markup for the assertion
            var postMarkup = page.Markup;
            
            
            // Reset
            
            // Update with the original data in database
            TestHelper.ProductService.SaveData(backupData);
            
            // Assert
            
            // Before searching, 1 post
            Assert.That(preMarkup.Contains("1 posts found"), Is.True);
            
            // After Searching, display No matching posts found.
            Assert.That(postMarkup.Contains("No matching post found."), Is.True);
        }
        
        /// <summary>
        /// Test the search function
        /// </summary>
        [Test]
        public void ContainsSearchText_Valid_TypeIn_Text_Should_Posts_Containing_Text()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            
            // Backup original database for resetting.
            var backupData = TestHelper.ProductService.GetAllData().First();
            // Make a fake post into database and try to search it
            var post = TestHelper.ProductService.GetAllData().First();
            post.Title = "test_title";
            post.Maker = "test_maker";
            post.Description = "test_des";
            TestHelper.ProductService.UpdateData(post);
            
            // Get the total post count for assertion
            var postCount = TestHelper.ProductService.GetAllData().Count();
            
            // Render the page
            var page = Render<ProductList>();
            // Get the markup for the assertion
            var preMarkup = page.Markup;
            
            // Act
            // Type text into the search box
            page.Find("input[id=\"post-search\"]").Input("test_title");
            // Get the markup for the assertion
            var titleMarkup = page.Markup;
            
            // Type text into the search box
            page.Find("input[id=\"post-search\"]").Input("test_maker");
            // Get the markup for the assertion
            var makerMarkup = page.Markup;
            
            // Type text into the search box
            page.Find("input[id=\"post-search\"]").Input("test_des");
            // Get the markup for the assertion
            var desMarkup = page.Markup;
            
            // Reset
            // Update with the original data in database
            TestHelper.ProductService.UpdateData(backupData);
            
            // Assert
            // Before searching, {postCount} posts
            Assert.That(preMarkup.Contains(postCount + " posts found"), Is.True);
            // After searching, 1 post
            Assert.That(titleMarkup.Contains("1 posts found"), Is.True);
            Assert.That(makerMarkup.Contains("1 posts found"), Is.True);
            Assert.That(desMarkup.Contains("1 posts found"), Is.True);
        }

        #endregion ContainsSearchText

        #region ClearSearch
        /// <summary>
        /// Test the clear button
        /// </summary>
        [Test]
        public void ClearSearch_Valid_TypeIn_Text_Should_Disappear()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            
            // Backup original database for resetting.
            var backupData = TestHelper.ProductService.GetAllData().First();
            // Make a fake post into database and try to search it
            var post = TestHelper.ProductService.GetAllData().First();
            post.Title = "test_title";
            TestHelper.ProductService.UpdateData(post);
            
            // Get the total post count for assertion
            var postCount = TestHelper.ProductService.GetAllData().Count();
            
            // Render the page
            var page = Render<ProductList>();
            
            // Type text into the search box
            page.Find("input[id=\"post-search\"]").Input("test_title");
            
            // Find the clear button
            var buttonList = page.FindAll("Button");
            var button = buttonList.First((m => m.OuterHtml.Contains("Clear")));
            
            // Get the markup for the assertion
            var preMarkup = page.Markup;
            
            // Act
            // Click the clear button
            button.Click();
            
            // Get the markup for the assertion
            var postMarkup = page.Markup;
            
            
            // Reset
            // Update with the original data in database
            TestHelper.ProductService.UpdateData(backupData);
            
            // Assert
            // Before searching, {postCount} posts
            Assert.That(preMarkup.Contains("1 posts found"), Is.True);
            // After searching, 1 post
            Assert.That(postMarkup.Contains(postCount + " posts found"), Is.True);
        }

        #endregion ClearSearch

        #region CloseProductModal

        /// <summary>
        ///  The "X" should close the modal
        /// </summary>
        [Test]
        public void CloseProductModal_Valid_Modal_Should_Be_Closed()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var id = "MoreInfoButton_jenlooper-cactus";

            var page = Render<ProductList>();
            
            // Find the Buttons (more info)
            var buttonList = page.FindAll("Button");
            
            // Find the one that matches the ID looking for and click it
            var button = buttonList.First((m => m.OuterHtml.Contains(id)));
            
            // Show the popup
            button.Click();
            
            // Get the markup to use for the assert
            var preMarkup = page.Markup;
            
            // Find the X button
            buttonList = page.FindAll("span");
            var xButton = buttonList.First((m => m.OuterHtml.Contains("xButton")));
            // Act
            xButton.Click();
            
            // Get the markup after close the model
            var postMarkup = page.Markup;

            // Reset
            // Not Needed

            // Assert
            Assert.That(preMarkup.Equals(postMarkup), Is.False);
        }
        #endregion CloseProductModal
        
        #region NextPage
        /// <summary>
        ///  Next button should change to next page
        /// </summary>
        [Test]
        public void NextPage_Valid_Should_Move_To_Next_Page()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var page = Render<ProductList>();
            
            // Find the Next button
            var buttonList = page.FindAll("Button");

            var button = buttonList.First((m => m.OuterHtml.Contains("NextButton")));
            
            // Get the markup to use for the assert
            var preMarkup = page.Markup;
            
            // Act
            button.Click();
            
            // Get the markup after close the model
            var postMarkup = page.Markup;

            // Reset
            // Not Needed

            // Assert
            Assert.That(preMarkup.Equals(postMarkup), Is.False);
            Assert.That(postMarkup.Contains("Page 2 of "), Is.True);
        }
        #endregion NextPage
        
        #region PreviousPage
        /// <summary>
        ///  Previous button should move to previous page
        /// </summary>
        [Test]
        public void PreviousPage_Valid_Should_Move_Back_To_Previous_Page()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var page = Render<ProductList>();
            
            // Find the Next button, and click it
            var buttonList = page.FindAll("Button");
            var nextButton = buttonList.First((m => m.OuterHtml.Contains("NextButton")));
            nextButton.Click();
            
            // Find the Previous button
            var prevButton = buttonList.First((m => m.OuterHtml.Contains("previousButton")));
            
            // Get the markup to use for the assert
            var preMarkup = page.Markup;
            
            // Act
            prevButton.Click();
            
            // Get the markup after close the model
            var postMarkup = page.Markup;

            // Reset
            // Not Needed

            // Assert
            Assert.That(preMarkup.Equals(postMarkup), Is.False);
            Assert.That(postMarkup.Contains("Page 1 of "), Is.True);
        }
        #endregion PreviousPage

        #region GetCurrentRating

        /// <summary>
        /// If the post has no vote yet, display "Vote" the current rating is 0.
        /// </summary>
        [Test]
        public void GetCurrentRating_Valid_VoteCount_0_Should_Set_Vote_And_0()
        {
            // Arrange
            
            // Put the first entry into ProductList, and set its VoteCount to 0
            var data = new ProductList();
            data.SelectedProduct = TestHelper.ProductService.GetAllData().First();
            data.SelectedProduct.Ratings = [];
            
            // Set the VoteCount and VoteLabel to arbitrary values for assertion
            data.VoteLabel = "test";
            data.CurrentRating = 999;
            

            // Act
            
            // The method should set VoteLabel to "Vote" and CurrentRating to 0
            data.GetCurrentRating();
            
            // Reset
            // Not needed

            // Assert
            Assert.That(data.VoteLabel, Is.EqualTo("Vote"));
            Assert.That(data.CurrentRating, Is.EqualTo(0));
        }
        
        #endregion GetCurrentRating
    }
}