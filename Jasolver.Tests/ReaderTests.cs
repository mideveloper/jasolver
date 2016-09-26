using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jasolver.Tests
{
    [TestClass]
    public class ReaderTests
    {
        [ClassInitialize]
        public static void InitTest(TestContext context)
        {
            Jasolver.JsonApi.Settings.AddDtoAssembliesNamespaceGroupWise("lms", "Jasolver.Tests", "Jasolver.Entities");

            Jasolver.JsonApi.Register();

          
        }
        [TestMethod]
        public void ReadArrayFormatJsonApi()
        {
            try
            {
                Jasolver.JsonApi.Decode(
               @"{
	""data"": [{
		""type"": ""articles"",
		""id"": ""1"",
		""attributes"": {
			""title"": ""JSON API paints my bikeshed!"",
            ""created-on"": ""2016-10-10""
		},
		""relationships"": {
			""author"": {
				""data"": {
					""type"": ""people"",
					""id"": ""9""
				}
			},  ""article-comments"": {
        
        ""data"": [
          { ""type"": ""comments"", ""id"": ""5"" },
          { ""type"": ""comments"", ""id"": ""12"" }
        ]
      }
		}}, {
		""type"": ""articles"",
		""id"": ""2"",
		""attributes"": {
			""title"": ""Article 2""
		},
		""relationships"": {
			""author"": {
				""data"": {
					""type"": ""people"",
					""id"": ""9""
				}
			},  ""article-comments"": {
        
        ""data"": [
          { ""type"": ""comments"", ""id"": ""5"" },
          { ""type"": ""comments"", ""id"": ""12"" }
        ]
      }
		}}],
""included"": [{
    ""type"": ""people"",
    ""id"": ""9"",
    ""attributes"": {
      ""first-name"": ""Dan"",
      ""last-name"": ""Gebhardt"",
      ""twitter"": ""dgeb""
    }},
{
    ""type"": ""comments"",
    ""id"": ""5"",
    ""attributes"": {
                ""body"": ""First!""
    },
    ""relationships"": {
                ""author"": {
                    ""data"": { ""type"": ""people"", ""id"": ""2"" }
                }
            },
    ""links"": {
                ""self"": ""http://example.com/comments/5""
    }
        }, {
    ""type"": ""comments"",
    ""id"": ""12"",
    ""attributes"": {
      ""body"": ""I like XML better""
    },
    ""relationships"": {
      ""author"": {
        ""data"": { ""type"": ""people"", ""id"": ""9"" }
      }
    },
    ""links"": {
      ""self"": ""http://example.com/comments/12""
    }
  }
  ]
		
	
}", "lms");
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }

        [TestMethod]
        public void ReadArrayFormatJsonApiInLoop1000Times()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    Jasolver.JsonApi.Decode(
              @"{
	""data"": [{
		""type"": ""articles"",
		""id"": ""1"",
		""attributes"": {
			""title"": ""JSON API paints my bikeshed!"",
            ""created-on"": ""2016-10-10""
		},
		""relationships"": {
			""author"": {
				""data"": {
					""type"": ""people"",
					""id"": ""9""
				}
			},  ""article-comments"": {
        
        ""data"": [
          { ""type"": ""comments"", ""id"": ""5"" },
          { ""type"": ""comments"", ""id"": ""12"" }
        ]
      }
		}}, {
		""type"": ""articles"",
		""id"": ""2"",
		""attributes"": {
			""title"": ""Article 2""
		},
		""relationships"": {
			""author"": {
				""data"": {
					""type"": ""people"",
					""id"": ""9""
				}
			},  ""article-comments"": {
        
        ""data"": [
          { ""type"": ""comments"", ""id"": ""5"" },
          { ""type"": ""comments"", ""id"": ""12"" }
        ]
      }
		}}],
""included"": [{
    ""type"": ""people"",
    ""id"": ""9"",
    ""attributes"": {
      ""first-name"": ""Dan"",
      ""last-name"": ""Gebhardt"",
      ""twitter"": ""dgeb""
    }},
{
    ""type"": ""comments"",
    ""id"": ""5"",
    ""attributes"": {
                ""body"": ""First!""
    },
    ""relationships"": {
                ""author"": {
                    ""data"": { ""type"": ""people"", ""id"": ""2"" }
                }
            },
    ""links"": {
                ""self"": ""http://example.com/comments/5""
    }
        }, {
    ""type"": ""comments"",
    ""id"": ""12"",
    ""attributes"": {
      ""body"": ""I like XML better""
    },
    ""relationships"": {
      ""author"": {
        ""data"": { ""type"": ""people"", ""id"": ""9"" }
      }
    },
    ""links"": {
      ""self"": ""http://example.com/comments/12""
    }
  }
  ]
		
	
}", "lms");
                   
                }
                Assert.IsTrue(true);
            }
            catch { Assert.Fail(); }
        }
    }
}
