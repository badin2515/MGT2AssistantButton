using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MGT2AssistantButton.Data
{
    [System.Serializable]
    public class GenreGameName
    {
        public int id;
        public string name;
        public List<string> names;
    }

    [System.Serializable]
    public class GameNamesData
    {
        public List<GenreGameName> genres;
    }

    public static class GameNamesDatabase
    {
        private static GameNamesData _data;
        private static bool _loaded = false;

        public static void Load()
        {
            if (_loaded) return;

            try
            {
                string pluginPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string jsonPath = Path.Combine(pluginPath, "Data", "GameNames.json");

                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    
                    // Manual parsing because JsonUtility doesn't handle nested structures well
                    _data = new GameNamesData { genres = new List<GenreGameName>() };
                    
                    // Simple JSON parsing
                    var lines = json.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    GenreGameName currentGenre = null;
                    
                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        
                        // Detect genre ID
                        if (trimmed.StartsWith("\"id\":"))
                        {
                            if (currentGenre != null && currentGenre.names != null && currentGenre.names.Count > 0)
                            {
                                _data.genres.Add(currentGenre);
                            }
                            currentGenre = new GenreGameName { names = new List<string>() };
                            var idStr = trimmed.Split(':')[1].TrimEnd(',').Trim();
                            currentGenre.id = int.Parse(idStr);
                        }
                        // Detect genre name
                        else if (trimmed.StartsWith("\"name\":") && currentGenre != null)
                        {
                            var nameStart = trimmed.IndexOf('"', 8) + 1;
                            var nameEnd = trimmed.LastIndexOf('"');
                            currentGenre.name = trimmed.Substring(nameStart, nameEnd - nameStart);
                        }
                        // Detect game names (inside "names" array)
                        else if (trimmed.StartsWith("\"") && trimmed.EndsWith("\",") || trimmed.EndsWith("\""))
                        {
                            // Extract name between quotes
                            var nameStart = trimmed.IndexOf('"') + 1;
                            var nameEnd = trimmed.LastIndexOf('"');
                            if (nameEnd > nameStart && currentGenre != null)
                            {
                                var gameName = trimmed.Substring(nameStart, nameEnd - nameStart);
                                if (!string.IsNullOrEmpty(gameName) && gameName != "genres" && gameName != "id" && gameName != "name" && gameName != "names")
                                {
                                    currentGenre.names.Add(gameName);
                                }
                            }
                        }
                    }
                    
                    // Don't forget last genre
                    if (currentGenre != null && currentGenre.names != null && currentGenre.names.Count > 0)
                    {
                        _data.genres.Add(currentGenre);
                    }
                    
                    _loaded = true;
                    Plugin.Logger.LogInfo($"Loaded game names database with {_data?.genres?.Count ?? 0} genres");
                }
                else
                {
                    Plugin.Logger.LogWarning($"GameNames.json not found at: {jsonPath}");
                    _data = new GameNamesData { genres = new List<GenreGameName>() };
                    _loaded = true;
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error loading GameNames.json: {ex.Message}");
                _data = new GameNamesData { genres = new List<GenreGameName>() };
                _loaded = true;
            }
        }

        // JsonUtility requires wrapper for arrays
        private static string WrapJson(string json)
        {
            return json;
        }

        public static string GetRandomNameForGenre(int genreId)
        {
            if (!_loaded) Load();

            if (_data == null || _data.genres == null)
                return null;

            // Find genre by ID
            GenreGameName genre = _data.genres.Find(g => g.id == genreId);
            
            if (genre == null || genre.names == null || genre.names.Count == 0)
                return null;

            // Return random name from that genre
            int randomIndex = UnityEngine.Random.Range(0, genre.names.Count);
            return genre.names[randomIndex];
        }

        public static bool HasNamesForGenre(int genreId)
        {
            if (!_loaded) Load();

            if (_data == null || _data.genres == null)
                return false;

            GenreGameName genre = _data.genres.Find(g => g.id == genreId);
            return genre != null && genre.names != null && genre.names.Count > 0;
        }
    }
}
