using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Jasolver.Entities;

namespace Jasolver.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public People Author { get; set; }

        public List<Comment> ArticleComments { get; set; }
    }
    public class People
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Twitter { get; set; }
    }
    public class Comment { public string Id { get; set; } public string Body { get; set; } public People Author { get; set; } }

}

namespace Jasolver.Tests
{
    
    [TestClass]
    public class WriterTests
    {
        static string[] includes;
        static Dictionary<string, string[]> fieldsDictionary;
      
        [ClassInitialize]
        public static void InitTest(TestContext context) {
            Jasolver.JsonApi.Settings.AddDtoAssembliesNamespaceGroupWise("lms", "Jasolver.Tests", "Jasolver.Entities");

            Jasolver.JsonApi.Register();

            includes = new string[] { "article-comments", "author", "article-comments.author" };
            fieldsDictionary = new Dictionary<string, string[]>();
            fieldsDictionary.Add("articles", new string[] { "title", "article-comments" });
            fieldsDictionary.Add("comments", new string[] { "author" });
            fieldsDictionary.Add("peoples", new string[] { "first-name" });
        }
      

       
        private static Article getArticleObj()
        {
            var comments = new List<Comment>();
            comments.Add(new Comment() { Body = "first comment", Id = "1", Author = new People() { Id = "2", FirstName = "Syed", LastName = "Ather" } });
            comments.Add(new Comment() { Body = "second comment", Id = "2", Author = new People() { Id = "2", FirstName = "Syed", LastName = "Ather" } });
            Article obj = new Article()
            {
                Id = 1,
                Author = new People() { Id = "1", FirstName = "Syed", LastName = "Azfar" },
                ArticleComments = comments,
                CreatedOn = DateTime.Now,
                Title = "My First Artilce"
            };
            return obj;

        }

        private static Article getArticleObjwithNullAuthor()
        {
            var comments = new List<Comment>();
            comments.Add(new Comment() { Body = "first comment", Id = "1", Author = new People() { Id = "2", FirstName = "Syed", LastName = "Ather" } });
            comments.Add(new Comment() { Body = "second comment", Id = "2", Author = new People() { Id = "2", FirstName = "Syed", LastName = "Ather" } });
            Article obj = new Article()
            {
                Id = 1,
                Author = null,
                ArticleComments = comments,
                CreatedOn = DateTime.Now,
                Title = "My First Artilce"
            };
            return obj;
        }

        private static Article getArticleObjwithNullAuthorAndComments()
        {
            Article obj = new Article()
            {
                Id = 1,
                Author = null,
                ArticleComments = null,
                CreatedOn = DateTime.Now,
                Title = "My First Artilce"
            };
            return obj;
        }

            
        private static List<Article> getListOfArticleObject()
        {
            List<Article> articles = new List<Article>();
            articles.Add(getArticleObj());
            articles.Add(getArticleObj());
            return articles;
        }

        [TestMethod]
        public void WriteListWhenRelationShipObjectsAreNotNull()
        {
            try
            {
                Jasolver.JsonApi.Encode<List<Article>>(getListOfArticleObject());
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void WriteListObjectWithIncludes()
        {
            try
            {
                Jasolver.JsonApi.Encode<List<Article>>(getListOfArticleObject(), includes);
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }
        [TestMethod]
        public void WriteListObjectWithIncludesAndFields()
        {
            try
            {
                Jasolver.JsonApi.Encode(getListOfArticleObject(), includes, fieldsDictionary);
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }
     
        [TestMethod]
        public void WriteObjectWithIncludesAndFieldsButObectInIncludeIsNull()
        {
            try
            {
                var obj = Jasolver.JsonApi.Encode<Article>(getArticleObjwithNullAuthorAndComments(), includes, fieldsDictionary);
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }

        [TestMethod]
        public void WriteObjectWithIncludesAndFields()
        {
            try
            {
                Jasolver.JsonApi.Encode<Article>(getArticleObj(), includes, fieldsDictionary);
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }

        [TestMethod]
        public void WriteObjectWithIncludes()
        {
            try
            {
                Jasolver.JsonApi.Encode<Article>(getArticleObj(), includes);
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }


        [TestMethod]
        public void WriteWhenRelationShipObjectsAreNull()
        {
            try
            {
                Jasolver.JsonApi.Encode<Article>(getArticleObjwithNullAuthorAndComments());
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }

        [TestMethod]
        public void WriteWhenRelationShipObjectsAreNotNull()
        {
            try
            {
                Jasolver.JsonApi.Encode<Article>(getArticleObj());
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void WriteWhenRelationShipOneObjectIsNullAndOtherHasValue()
        {
            try
            {
                Jasolver.JsonApi.Encode<Article>(getArticleObjwithNullAuthor());
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }

        [TestMethod]
        public void WriteWhenRelationShipOneObjectIsNullAndOtherHasValueInLoop1000Times()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    Jasolver.JsonApi.Encode<Article>(getArticleObjwithNullAuthor());
                }
                    
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }
    }
}
