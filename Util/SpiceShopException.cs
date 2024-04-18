using System;

namespace SpiceShop.Util;

public class SpiceShopException : Exception
{
    public SpiceShopException(string message = null) : base(message)
    {
    }
}