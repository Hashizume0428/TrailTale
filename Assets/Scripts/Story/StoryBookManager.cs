using System;
using System.IO;
using UnityEngine;
using AILibrary;

namespace StoryLibrary
{
    [Serializable]
    public class Story
    {
        public string title;
        public string description;
        public string[] options;
        public int selectedOptionIndex;
        public string result;
    }

    /// <summary>
    /// ResponseContentとプレイヤーの選択をStoryに変換するクラスです。
    /// </summary>
    public static class StoryParser
    {
        public static Story ParseFromResponseContent(ResponseContent responseContent, int selectedOptionIndex)
        {
            Story story = new Story
            {
                title = responseContent.stage,
                description = responseContent.description,
                options = new string[responseContent.options.Length],
                selectedOptionIndex = selectedOptionIndex,
                result = responseContent.options[selectedOptionIndex].result
            };

            for (int i = 0; i < responseContent.options.Length; i++)
            {
                story.options[i] = responseContent.options[i].title;
            }

            return story;
        }
    }

    /// <summary>
    /// ストーリーを管理しておく本の管理クラスです。
    /// このクラスは、指定した本のページに保存・読み込みする機能を提供します。
    /// </summary>
    public static class StoryBookManager
    {
#if UNITY_EDITOR
        private static string basePath => Path.Combine(Application.dataPath, "Data/Bookshelf");
#elif UNITY_ANDROID
        private static string basePath => Path.Combine(Application.persistentDataPath, "Bookshelf");
#endif

        /// <summary>
        /// 指定した本のページにストーリーを保存します。
        /// ページ番号は自動的に決定され、存在しない場合は新しいページとして保存されます。
        /// </summary>
        /// <param name="bookName"></param>
        /// <param name="story"></param>
        public static void SavePage(string bookName, Story story)
        {
            string dirPath = Path.Combine(basePath, bookName);
            Directory.CreateDirectory(dirPath);

            // ページ番号を決定
            int pageNumber = Directory.GetFiles(dirPath, "Page_*.json").Length + 1;

            string json = JsonUtility.ToJson(story, true);
            string filePath = Path.Combine(dirPath, $"Page_{pageNumber:D2}.json");
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 指定した本の指定したページにストーリーを保存します
        /// </summary>
        /// <param name="bookName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="story"></param>
        public static void SavePage(string bookName, int pageNumber, Story story)
        {
            string dirPath = Path.Combine(basePath, bookName);
            Directory.CreateDirectory(dirPath);

            string json = JsonUtility.ToJson(story, true);
            string filePath = Path.Combine(dirPath, $"Page_{pageNumber:D2}.json");
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 指定した本のページを読み込みます
        /// </summary>
        /// <param name="bookName"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static Story LoadPage(string bookName, int pageNumber)
        {
            string filePath = Path.Combine(basePath, bookName, $"Page_{pageNumber:D2}.json");
            if (!File.Exists(filePath)) return null;

            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Story>(json);
        }

        /// <summary>
        /// 存在する全ての本の名前を取得します
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllBooks()
        {
            if (!Directory.Exists(basePath)) return new string[0];
            return Directory.GetDirectories(basePath);
        }

        /// <summary>
        /// 指定した本の全てのページファイルのパスを取得します
        /// </summary>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public static string[] GetPagesInBook(string bookName)
        {
            string dirPath = Path.Combine(basePath, bookName);
            if (!Directory.Exists(dirPath)) return new string[0];
            return Directory.GetFiles(dirPath, "Page_*.json");
        }
    }

}

