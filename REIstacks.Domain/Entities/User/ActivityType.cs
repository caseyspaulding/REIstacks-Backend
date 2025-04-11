namespace REIstacks.Domain.Entities.User;

public enum ActivityType
{
    SignUp,
    SignIn,
    SignOut,
    UpdatePassword,
    DeleteAccount,
    UpdateAccount,
    CreateTeam,
    RemoveTeamMember,
    InviteTeamMember,
    AcceptInvitation
}