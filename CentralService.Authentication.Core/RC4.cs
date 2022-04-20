using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.Core
{
    public static class RC4
    {
        public static byte[] Compute(string Challenge, string GameKey)
        {
            byte[] ChallengeBytes = Encoding.UTF8.GetBytes(Challenge);
            byte[] GameKeyBytes = Encoding.UTF8.GetBytes(GameKey);
            byte[] ChallengeResult = new byte[Challenge.Length + 1];

            byte[] Table = new byte[256];
            for (int i = 0; i < Table.Length; i++)
                Table[i] = (byte)i;

            int TableIndex = 0;
            for (int i = 0; i < Table.Length; i++)
            {
                TableIndex = (TableIndex + Table[i] + GameKeyBytes[i % GameKeyBytes.Length]) & 0xFF;
                byte TableValue = Table[TableIndex];
                Table[TableIndex] = Table[i];
                Table[i] = TableValue;
            }

            int TableIndex1 = 0;
            int TableIndex2 = 0;
            for (int i = 0; i < ChallengeBytes.Length; i++)
            {
                TableIndex1 = (TableIndex1 + ChallengeBytes[i] + 1) & 0xFF;
                TableIndex2 = (TableIndex2 + Table[TableIndex1]) & 0xFF;

                byte Buffer = Table[TableIndex1];
                Table[TableIndex1] = Table[TableIndex2];
                Table[TableIndex2] = Buffer;

                ChallengeResult[i] = (byte)((ChallengeBytes[i] ^ Table[(Table[TableIndex1] + Table[TableIndex2]) & 0xFF]) & 0xFF);
            }
            ChallengeResult[ChallengeResult.Length - 1] = 0;
            return ChallengeResult;
        }
    }
}
