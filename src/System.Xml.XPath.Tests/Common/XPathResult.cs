﻿namespace XPathTests.Common
{
    public class XPathResult
    {
        public XPathResultToken[] Results { get; set; }
        public int CurrentPosition { get; set; }

        public XPathResult(int currentPosition, params XPathResultToken[] resultTokens)
        {
            CurrentPosition = currentPosition;
            Results = resultTokens;
        }
    }
}