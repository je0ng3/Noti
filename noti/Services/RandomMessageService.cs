using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noti.Services
{
    internal class RandomMessageService
    {
        private readonly List<string> _messages = new()
        {
            "물 마셔라 인간",
            "스트레칭 좀 해라 인간",
            "눈 좀 쉬어라 인간",
            "잠깐 산책 좀 해라 인간",
            "심호흡 좀 해라 인간",
            "점점 자세가 틀어지고 못생겨지고",
            "너 표정 좀 봐라",
            "사실 고양이는.. 귀엽다(TRUE)",
            "사실 개발자는...",
            "사실 고슴도치는... 뾰족하다",
         
        };

        private readonly Random _random = new();

        public string GetRandomMessage()
        {
            int index = _random.Next(_messages.Count);
            return _messages[index];
        }
    }
}
