using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObjects;

public partial class Tag
{
    public int TagID { get; set; }

    public string? TagName { get; set; }

    public string? Note { get; set; }

    [JsonIgnore]
    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
