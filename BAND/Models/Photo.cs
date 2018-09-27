using Newtonsoft.Json;

namespace BAND.Models
{
    public class Photo
    {
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("author")]
        public Author Author { get; set; }
        [JsonProperty("photo_album_key")]
        public string PhotoAlbumKey { get; set; }
        [JsonProperty("photo_key")]
        public string PhotoKey { get; set; }
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty("emotion_count")]
        public int EmotionCount { get; set; }
        [JsonProperty("is_video_thumbnail")]
        public bool IsVideoThumbnail { get; set; }
    }
}
