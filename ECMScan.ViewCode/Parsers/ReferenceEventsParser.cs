using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISBLScan.ViewCode
{
    static class ReferenceEventsParser
    {
        public static Dictionary<Char, String> SectionCodeToName = new Dictionary<char, string>()
        {
            {'Ш', "Карточка"},
            {'Т', "Таблица"},
            {'С', "Таблица 2"},
            {'Р', "Таблица 3"},
            {'О', "Таблица 4"},
            {'Н', "Таблица 5"},
            {'М', "Таблица 6"},
            {'Q', "Таблица 7"},
            {'W', "Таблица 8"},
            {'U', "Таблица 9"},
            {'R', "Таблица 10"},
            {'I', "Таблица 11"},
            {'Y', "Таблица 12"},
            {'B', "Таблица 13"},
            {'H', "Таблица 14"},
            {'L', "Таблица 15"},
            {'M', "Таблица 16"},
            {'N', "Таблица 17"},
            {'P', "Таблица 18"},
            {'O', "Таблица 19"},
            {'S', "Таблица 20"},
            {'T', "Таблица 21"},
            {'V', "Таблица 22"},
            {'X', "Таблица 23"},
            {'Z', "Таблица 24"},
            {'К', "Действие"},
            {'A', "Действие"},
            {'C', "Карточка"},
            {'D', "Таблица"},
            {'E', "Таблица 2"},
            {'F', "Таблица 3"},
            {'G', "Таблица 4"},
            {'J', "Таблица 5"},
            {'K', "Таблица 6"}
        };

        static Dictionary<string, String> EventCodePartToTitle = new Dictionary<string, string>()
        {
            {"DIALOG.CREATE", "Диалог. Создание"},
            {"DIALOG.VALID_CLOSE_WITH_RESULT", "Диалог. Закрытие Возможность"},
            {"DIALOG.CLOSE", "Диалог. Закрытие"},
            {"FORM.DIALOG_SHOW", "Форма. Показ"},
            {"FORM.DIALOG_HIDE", "Форма. Скрытие"},

            {"OPEN", "Открытие"},
            {"CLOSE", "Закрытие"},
            {"DATASET", "Набор данных"},
            {"CARD", "Запись"},
            {"VALID_DELETE", "Удаление возможность"},
            {"BEFORE_DELETE", "Удаление до"},
            {"AFTER_DELETE", "Удаление после"},
            {"BEFORE_INSERT", "Добавление до"},
            {"AFTER_INSERT", "Добавление после"},
            {"VALID_UPDATE", "Сохранение возможность"},
            {"BEFORE_UPDATE", "Сохранение до"},
            {"AFTER_UPDATE", "Сохранение после"},
            {"BEFORE_CANCEL", "Отмена до"},
            {"AFTER_CANCEL", "Отмена после"},
            {"FORM", "Форма-карточка"},
            {"SHOW", "Показ"},
            {"HIDE", "Скрытие"},
            {"TABLE", "Таблица"}
        };

        static string[] EventIdsRegExPatterns =
        {
            "DATASET{9AFC8FC7-30C4-4076-9076-6E09A49B791C}.[A-Z А-Я_]+",
            "CARD{2147B5A6-496E-4EFF-88D9-78970D889F1F}.[A-Z А-Я_]+",
            "FORM{B28D55C1-651A-46C9-AD4E-50E73EF213A8}.[A-Z А-Я_]+",
            "TABLE{D402E843-74B2-4DC1-BFFD-DE677B48452C}[0-9]*.[A-Z А-Я_]+",
            "DIALOG{3AA220D8-D906-4914-8586-F534A4C3767E}.[A-Z А-Я_]+",
            "КАРТОЧКА.[A-Z А-Я_]+",
            "НАБОР ДАННЫХ.[A-Z А-Я_]+",
            "ТАБЛИЦА[0-9]*.[A-Z А-Я_]+",
            "DATASET.[A-Z А-Я_]+",
            "CARD.[A-Z А-Я_]+",
            "FORM.[A-Z А-Я_]+",
            "TABLE[0-9]*.[A-Z А-Я_]+"
        };
        public static void ParseEvents(string eventsText, IsbNode parent)
        {
            var pattern = $"^({String.Join("|", EventIdsRegExPatterns)})";
            var events = Regex.Split(eventsText, pattern, RegexOptions.Multiline);
            if (events.Length > 0)
            {
                var eventsNode = new IsbNode("События");
                IsbNode eventNode = null;
                foreach (var eventText in events)
                {
                    if (!String.IsNullOrWhiteSpace(eventText))
                    {
                        if (Regex.IsMatch(eventText, pattern))
                        {
                            var eventName = Regex.Replace(eventText, "\\{[-\\w]+\\}", "");
                            foreach (var part in EventCodePartToTitle.Keys)
                            {
                                eventName = eventName.Replace(part, EventCodePartToTitle[part]);
                            }
                            eventNode = new IsbNode(eventName);
                        }
                        else
                        {
                            eventNode.Text = eventText.Substring(2);
                            eventsNode.Nodes.Add(eventNode);
                        }
                    }
                }
                parent.Nodes.Add(eventsNode);
            }
        }
    }
}
