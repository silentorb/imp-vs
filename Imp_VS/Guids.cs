// Guids.cs
// MUST match guids.h
using System;

namespace SilentOrb.Imp_VS
{
    static class GuidList
    {
        public const string guidImp_VSPkgString = "4067d230-7745-41b8-966b-cabec1d83938";
        public const string guidImp_VSCmdSetString = "ad0ed5af-5bde-43ce-8c5d-df49afe53675";

        public static readonly Guid guidImp_VSCmdSet = new Guid(guidImp_VSCmdSetString);
    };
}