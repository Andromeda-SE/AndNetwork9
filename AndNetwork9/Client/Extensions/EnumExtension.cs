using System;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Client.Extensions
{
    public static class EnumExtension
    {
        public static string GetIconStyle(this SortType sortType, bool reverse)
        {
            return "bi-sort"
                   + sortType switch
                   {
                       SortType.Default => string.Empty,
                       SortType.Alphabet => "-alpha",
                       SortType.Numeric => "-numeric",
                       _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null),
                   }
                   + "-down"
                   + (reverse ? "-alt" : string.Empty);
        }

        public static string GetColorStyle(this Direction direction)
        {
            return direction switch
            {
                Direction.Reserve => "#dee2e6",
                Direction.None => "#dc3545",
                Direction.Training => "#ffc107",
                Direction.Infrastructure => "#fd7e14",
                Direction.Research => "#198754",
                Direction.Military => "#0d6efd",
                Direction.Agitation => "#6f42c1",
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static string GetName(this RepoNodeEditor.VersionLevel level)
        {
            return level switch
            {
                RepoNodeEditor.VersionLevel.Version => "Версия",
                RepoNodeEditor.VersionLevel.Modification => "Модификация",
                RepoNodeEditor.VersionLevel.Prototype => "Прототип",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
            };
        }

        public static string GetName(this FastMemberSelector.FastEditType editType)
        {
            return editType switch
            {
                FastMemberSelector.FastEditType.One => "Единичный выбор",
                FastMemberSelector.FastEditType.Squad => "Отряд",
                FastMemberSelector.FastEditType.Direction => "Направление (общность игроков)",
                FastMemberSelector.FastEditType.VoteMembers => "Участники голосования",
                FastMemberSelector.FastEditType.VoteVoted => "Проголосовавшие участники голосования",
                FastMemberSelector.FastEditType.SquadCommanders => "Командующие отрядами",
                FastMemberSelector.FastEditType.Advisors => "Советники",
                FastMemberSelector.FastEditType.All => "Весь клан",
                _ => throw new ArgumentOutOfRangeException(nameof(editType), editType, null),
            };
        }
    }
}