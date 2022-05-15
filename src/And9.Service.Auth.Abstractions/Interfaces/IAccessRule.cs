using System.Linq.Expressions;
using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Service.Auth.Abstractions.Interfaces;

public interface IAccessRule : IId
{
    Rank MinRank { get; }
    //Direction[] Directions { get; }
    public short? SquadNumber { get; }
    public short? SquadPartNumber { get; }

    List<int> AllowedMembersIds { get; }

    public Expression<Func<IMember, bool>> HasAccessExpression => member => member.Rank == Rank.FirstAdvisor
                                                                            || AllowedMembersIds.Any(
                                                                                x => x == member.Id)
                                                                            /*|| Directions.Any(
                                                                                x => x == member.Direction)*/
                                                                            && member.Rank >= MinRank
                                                                            && (
                                                                                SquadNumber == null
                                                                                && SquadPartNumber == null
                                                                                || SquadNumber != null
                                                                                && SquadPartNumber == null
                                                                                && SquadNumber == member.SquadPartNumber
                                                                                || SquadNumber != null
                                                                                && SquadPartNumber != null
                                                                                && SquadNumber == member.SquadPartNumber
                                                                                && SquadPartNumber
                                                                                == member.SquadPartNumber
                                                                            );
}