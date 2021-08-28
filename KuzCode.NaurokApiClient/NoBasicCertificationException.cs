using System;

namespace KuzCode.NaurokApiClient
{
    /// <summary>
    /// Представляет ошибку, которая возникает из-за отсутствя у учителя базовой сертефикации
    /// </summary>
    public class NoBasicCertificationException : Exception
    {
        public override string Message => "The teacher has not basic certification";
    }
}
