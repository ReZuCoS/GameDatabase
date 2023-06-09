﻿using API.Utils;
using OtpNet;
using System.ComponentModel.DataAnnotations;

namespace API.Model.DataTransferObjects
{
    /// <summary>
    /// New 2fa data
    /// </summary>
    public class TwoFactorDto
    {
        /// <summary>
        /// New 2fa data constructor
        /// </summary>
        public TwoFactorDto()
        {
            var recoveries = new string[6];

            for (int i = 0; i < 6; i++)
            {
                recoveries[i] = HashGenerator.GenerateSalt(10);
            }

            Key = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(25));
            Recoveries = recoveries;
        }

        /// <summary>
        /// TOTP key
        /// </summary>
        [Required]
        public string Key { get; set; }
        
        /// <summary>
        /// TOTP recovery codes
        /// </summary>
        [Required]
        public string[] Recoveries { get; set; } = Array.Empty<string>();
    }
}
