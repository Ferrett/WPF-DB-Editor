using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopWpf
{
    public static class Routes
    {
        public const string APIurl = @"https://xhvlop3q7v55snb2tvjh7dt57a0jswko.lambda-url.eu-north-1.on.aws";
        public const string DefaultLogoName = "Dummy.png";

        public const string GetRequest = "GetAll";
        public const string GetByIDRequest = "Get";
        public const string DeleteRequest = "Delete";
        public const string PostRequest = "Post";

        // Developer
        public const string PutLogoRequest = "PutLogo";
        public const string PutNameRequest = "PutName";

        // Game;
        public const string PutAchievementsCountRequest = "PutAchievementsCount";
        public const string PutPriceRequest = "PutPrice";

        // Game stats
        public const string PutGottenAchievementsRequest = "PutGottenAchievements";
        public const string PutHoursPlayedRequest = "PutHoursPlayed";
        public const string PutGameLaunchedRequest = "PutGameLaunched";

        // Review
        public const string PutTextRequest = "PutText";
        public const string PutApprovalRequest = "PutApproval";

        // User
        public const string PutLoginRequest = "PutLogin";
        public const string PutPasswordRequest = "PutPassword";
        public const string PutNicknameRequest = "PutNickname";
        public const string PutEmailRequest = "PutEmail";
        public const string PutMoneyOnAccountRequest = "PutMoneyOnAccount";
    }
}
