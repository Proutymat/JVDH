using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class SRTParser
{
    public static List<SubtitleEntry> Parse(string srtText)
    {
        List<SubtitleEntry> subtitles = new();

        if (string.IsNullOrWhiteSpace(srtText))
        {
            Debug.LogError("[SRTParser] Le texte SRT est vide ou null.");
            return subtitles;
        }

        string[] blocks = srtText.Split(
            new string[] { "\r\n\r\n", "\n\n" },
            StringSplitOptions.RemoveEmptyEntries
        );

        Debug.Log($"[SRTParser] {blocks.Length} blocs détectés.");

        for (int i = 0; i < blocks.Length; i++)
        {
            string block = blocks[i].Trim();

            if (string.IsNullOrWhiteSpace(block))
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} vide ignoré.");
                continue;
            }

            string[] lines = block.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            if (lines.Length < 3)
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} invalide : moins de 3 lignes.");
                continue;
            }

            if (!lines[1].Contains(" --> "))
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} sans timestamp valide : {lines[1]}");
                continue;
            }

            string[] times = lines[1].Split(new string[] { " --> " }, StringSplitOptions.None);

            if (times.Length != 2)
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} timestamps mal formés.");
                continue;
            }

            if (!TryParseTime(times[0], out double startTime))
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} startTime invalide : {times[0]}");
                continue;
            }

            if (!TryParseTime(times[1], out double endTime))
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} endTime invalide : {times[1]}");
                continue;
            }

            if (endTime <= startTime)
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} durée nulle ou négative ignorée.");
                continue;
            }

            string text = string.Join("\n", lines, 2, lines.Length - 2).Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Debug.LogWarning($"[SRTParser] Bloc {i} sans texte.");
                continue;
            }

            subtitles.Add(new SubtitleEntry
            {
                startTime = startTime,
                endTime = endTime,
                text = text
            });
        }

        Debug.Log($"[SRTParser] {subtitles.Count} sous-titres valides chargés.");

        return subtitles;
    }

    private static bool TryParseTime(string time, out double seconds)
    {
        seconds = 0;

        if (string.IsNullOrWhiteSpace(time))
            return false;

        time = time.Trim().Replace(',', '.');

        bool success = TimeSpan.TryParseExact(
            time,
            @"hh\:mm\:ss\.fff",
            CultureInfo.InvariantCulture,
            out TimeSpan ts
        );

        if (!success)
            return false;

        seconds = ts.TotalSeconds;
        return true;
    }
}