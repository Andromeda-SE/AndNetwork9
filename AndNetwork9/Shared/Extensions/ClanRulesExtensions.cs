using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Extensions
{
    public static class ClanRulesExtensions
    {
        public const int ADVISOR_TERM_DAYS = 90;
        public const int VOTING_DAYS = 5;
        public const int ANNOUNCEMENT_DAYS = 5;
        public const int MIN_DIRECTION_CHANGE_DAYS = 5;

        public static readonly IReadOnlyDictionary<Rank, int> RankPoints = new ReadOnlyDictionary<Rank, int>(
            new Dictionary<Rank, int>(new[]
            {
                new KeyValuePair<Rank, int>(Rank.Neophyte, 0),
                new KeyValuePair<Rank, int>(Rank.Trainee, 1),
                new KeyValuePair<Rank, int>(Rank.Assistant, 5),
                new KeyValuePair<Rank, int>(Rank.JuniorEmployee, 15),
                new KeyValuePair<Rank, int>(Rank.Employee, 25),
                new KeyValuePair<Rank, int>(Rank.SeniorEmployee, 50),
                new KeyValuePair<Rank, int>(Rank.Specialist, 75),
                new KeyValuePair<Rank, int>(Rank.Defender, 100),
            }));

        public static Rank GetRank(this IEnumerable<Award> awards)
        {
            int result = awards.Sum(x => (int)x.Type);
            return RankPoints.Where(x => x.Value <= result).OrderByDescending(x => x.Value).First().Key;
        }

        public static string? GetRankIcon(this Rank rank)
        {
            return rank switch
            {
                Rank.Outcast => null,
                Rank.Enemy => null,
                Rank.Guest => null,
                Rank.Diplomat => null,
                Rank.Ally => null,
                Rank.Candidate => null,
                Rank.None => null,
                Rank.Neophyte => "⦁",
                Rank.Trainee => "❮❮❮",
                Rank.Assistant => "❮❮",
                Rank.JuniorEmployee => "❮",
                Rank.Employee => "❙❙❙",
                Rank.SeniorEmployee => "❙❙",
                Rank.Specialist => "❙",
                Rank.Defender => "⛉",
                Rank.Advisor => "△",
                Rank.FirstAdvisor => "▲",
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null),
            };
        }

        public static string? GetAsciiRankIcon(this Rank rank)
        {
            return rank switch
            {
                Rank.Outcast => null,
                Rank.Enemy => null,
                Rank.Guest => null,
                Rank.Diplomat => null,
                Rank.Ally => null,
                Rank.Candidate => null,
                Rank.None => null,
                Rank.Neophyte => "O",
                Rank.Trainee => "VVV",
                Rank.Assistant => "VV",
                Rank.JuniorEmployee => "V",
                Rank.Employee => "III",
                Rank.SeniorEmployee => "II",
                Rank.Specialist => "I",
                Rank.Defender => "D",
                Rank.Advisor => "A",
                Rank.FirstAdvisor => "1A",
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null),
            };
        }

        public static string GetName(this Direction department)
        {
            return department switch
            {
                Direction.Reserve => "Резерв",
                Direction.None => "Н/Д",
                Direction.Training => "Обучение и тренировка",
                Direction.Infrastructure => "Развитие и инфраструктура",
                Direction.Research => "Исследования и разработка",
                Direction.Military => "Атака и оборона",
                Direction.Agitation => "Агитация и внешние связи",
                _ => throw new ArgumentOutOfRangeException(nameof(department), department, null),
            };
        }

        public static string GetRankName(this Rank rank)
        {
            return rank switch
            {
                Rank.Outcast => "Изганнник",
                Rank.Guest => "Гость",
                Rank.Diplomat => "Дипломат",
                Rank.Ally => "Союзник",
                Rank.Candidate => "Кандидат",
                Rank.None => "",
                Rank.Neophyte => "Неофит",
                Rank.Trainee => "Стажёр",
                Rank.Assistant => "Ассистент",
                Rank.JuniorEmployee => "Младший сотрудник",
                Rank.Employee => "Сотрудник",
                Rank.SeniorEmployee => "Старший сорудник",
                Rank.Specialist => "Специалист",
                Rank.Defender => "Защитник",
                Rank.Advisor => "Советник",
                Rank.FirstAdvisor => "Первый советник",
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null),
            };
        }

        public static string GetTypeName(this AwardType type)
        {
            return type switch
            {
                AwardType.None => string.Empty,
                AwardType.Bronze => "Бронза",
                AwardType.Silver => "Серебро",
                AwardType.Gold => "Золото",
                AwardType.Hero => "Звание героя клана",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        public static string GetVoteName(this MemberVote vote)
        {
            return vote switch
            {
                MemberVote.Invalid => string.Empty,
                MemberVote.NoVote => "Голос не отдан",
                MemberVote.Accept => "За",
                MemberVote.Decline => "Против",
                MemberVote.Abstained => "Равнозначно",
                MemberVote.NeedMoreInformation => "Необходимо больше информации",
                _ => throw new ArgumentOutOfRangeException(nameof(vote), vote, null),
            };
        }

        public static string GetVoteResult(this MemberVote vote)
        {
            return vote switch
            {
                MemberVote.Invalid => string.Empty,
                MemberVote.NoVote => "Голосование идет",
                MemberVote.Accept => "Принято",
                MemberVote.Decline => "Отклюнено",
                MemberVote.Abstained => "Статус-кво",
                MemberVote.NeedMoreInformation => "Недостаточно информации",
                _ => throw new ArgumentOutOfRangeException(nameof(vote), vote, null),
            };
        }

        public static string GetAwardSymbol(this AwardType awardType)
        {
            return awardType switch
            {
                AwardType.None => string.Empty,
                AwardType.Bronze => "\U0001F7EB",
                AwardType.Silver => "\U00002B1C",
                AwardType.Gold => "\U0001F7E8",
                AwardType.Hero => "\U0001F7E6",
                _ => throw new ArgumentOutOfRangeException(nameof(awardType), awardType, null),
            };
        }

        public static string GetString(this RepoType type)
        {
            return type switch
            {
                RepoType.None => "Н/Д",
                RepoType.Blueprint => "Чертёж",
                RepoType.Script => "Скрипт",
                RepoType.World => "Мир",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }
}