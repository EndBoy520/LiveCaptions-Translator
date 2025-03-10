﻿using System.Diagnostics;

using LiveCaptionsTranslator.models;

namespace LiveCaptionsTranslator.utils
{
    public static class Translator
    {
        public static event Action? TranslationLogged;

        public static async Task<string> Translate(string text)
        {
            string translatedText;
            try
            {
#if DEBUG
                var sw = Stopwatch.StartNew();
#endif
                translatedText = await TranslateAPI.TranslateFunc(text);
#if DEBUG
                sw.Stop();
                translatedText = $"[{sw.ElapsedMilliseconds} ms] " + translatedText;
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Translation failed: {ex.Message}");
                return $"[Translation Failed] {ex.Message}";
            }
            return translatedText;
        }

        public static async Task Log(string originalText, string translatedText, 
            Setting? setting = null, bool isOverWrite = false)
        {
            string targetLanguage, apiName;
            if (setting != null)
            {
                targetLanguage = App.Settings.TargetLanguage;
                apiName = App.Settings.ApiName;
            } 
            else
            {
                targetLanguage = "N/A";
                apiName = "N/A";
            }

            try
            {
                if (isOverWrite)
                    await SQLiteHistoryLogger.DeleteLatestTranslation();
                await SQLiteHistoryLogger.LogTranslation(originalText, translatedText, targetLanguage, apiName);
                TranslationLogged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Logging history failed: {ex.Message}");
            }
        }

        public static async Task LogOnly(string originalText, bool isOverWrite = false)
        {
            try
            {
                if (isOverWrite)
                    await SQLiteHistoryLogger.DeleteLatestTranslation();
                await SQLiteHistoryLogger.LogTranslation(originalText, "N/A", "N/A", "LogOnly");
                TranslationLogged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Logging history failed: {ex.Message}");
            }
        }
    }
}