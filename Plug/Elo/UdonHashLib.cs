﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// -*- coding: utf-8 -*-
/*
 * Code modified by Oikki
 * check original at https://github.com/Gorialis/vrchat-udon-hashlib/blob/main/Assets/Gorialis/UdonHashLib/UdonSharp/UdonHashLib.cs
MIT License

Copyright (c) 2021-present Devon (Gorialis) R

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

//cheese using it now

using System;
using System.Linq;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UdonSharpEditor;
#endif


[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UdonHashLib : UdonSharpBehaviour
{
	// Udon does not support UTF-8 or expose System.Text.Encoding so we must implement this ourselves
	private static byte[] ToUTF8(char[] characters)
	{
		byte[] buffer = new byte[characters.Length * 4];

		int writeIndex = 0;
		for (int i = 0; i < characters.Length; i++)
		{
			uint character = characters[i];

			if (character < 0x80)
			{
				buffer[writeIndex++] = (byte)character;
			}
			else if (character < 0x800)
			{
				buffer[writeIndex++] = (byte)(0b11000000 | ((character >> 6) & 0b11111));
				buffer[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
			}
			else if (character < 0x10000)
			{
				buffer[writeIndex++] = (byte)(0b11100000 | ((character >> 12) & 0b1111));
				buffer[writeIndex++] = (byte)(0b10000000 | ((character >> 6) & 0b111111));
				buffer[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
			}
			else
			{
				buffer[writeIndex++] = (byte)(0b11110000 | ((character >> 18) & 0b111));
				buffer[writeIndex++] = (byte)(0b10000000 | ((character >> 12) & 0b111111));
				buffer[writeIndex++] = (byte)(0b10000000 | ((character >> 6) & 0b111111));
				buffer[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
			}
		}

		// We do this to truncate off the end of the array
		// This would be a lot easier with Array.Resize, but Udon once again does not allow access to it.
		byte[] output = new byte[writeIndex];

		for (int i = 0; i < writeIndex; i++)
			output[i] = buffer[i];

		return output;
	}
	private static string BytesToString(byte[] bytes)
	{
		string output = "";
		foreach (var item in bytes)
		{
			output += item.ToString("x2");
		}
		return output;
	}

	public static byte[] HexStringToByteArray(string hex)
	{
		// 创建一个长度为 hex.Length / 2 的字节数组
		byte[] bytes = new byte[hex.Length / 2];

		// 循环遍历每两个字符并转换为一个字节
		for (int i = 0; i < hex.Length; i += 2)
		{
			// 取两个字符并将其转换为一个字节
			bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}

		return bytes;
	}

	private static string MD5_Core(byte[] payload_bytes, ulong[] init, ulong[] constants, int[] shifts, ulong size_mask, int word_size, int chunk_modulo, int appended_length, int round_count, string output_format, int output_segments)
	{
		int word_bytes = word_size / 8;

		// Working variables a0->d0
		ulong[] working_variables = new ulong[4];
		init.CopyTo(working_variables, 0);

		byte[] input = new byte[chunk_modulo];
		ulong[] message_schedule = new ulong[16];

		// Each 64-byte/512-bit chunk
		// 64 bits/8 bytes are required at the end for the bit size
		for (int chunk_index = 0; chunk_index < payload_bytes.Length + appended_length + 1; chunk_index += chunk_modulo)
		{
			int chunk_size = Mathf.Min(chunk_modulo, payload_bytes.Length - chunk_index);
			int schedule_index = 0;

			// Buffer message
			for (; schedule_index < chunk_size; ++schedule_index)
				input[schedule_index] = payload_bytes[chunk_index + schedule_index];
			// Append a 1-bit if not an even chunk
			if (schedule_index < chunk_modulo && chunk_size >= 0)
				input[schedule_index++] = 0b10000000;
			// Pad with zeros until the end
			for (; schedule_index < chunk_modulo; ++schedule_index)
				input[schedule_index] = 0x00;
			// If the chunk is less than 56 bytes, this will be the final chunk containing the data size in bits
			if (chunk_size < chunk_modulo - appended_length)
			{
				ulong bit_size = (ulong)payload_bytes.Length * 8ul;
				input[chunk_modulo - 8] = Convert.ToByte((bit_size >> 0x00) & 0xFFul);
				input[chunk_modulo - 7] = Convert.ToByte((bit_size >> 0x08) & 0xFFul);
				input[chunk_modulo - 6] = Convert.ToByte((bit_size >> 0x10) & 0xFFul);
				input[chunk_modulo - 5] = Convert.ToByte((bit_size >> 0x18) & 0xFFul);
				input[chunk_modulo - 4] = Convert.ToByte((bit_size >> 0x20) & 0xFFul);
				input[chunk_modulo - 3] = Convert.ToByte((bit_size >> 0x28) & 0xFFul);
				input[chunk_modulo - 2] = Convert.ToByte((bit_size >> 0x30) & 0xFFul);
				input[chunk_modulo - 1] = Convert.ToByte((bit_size >> 0x38) & 0xFFul);
			}

			// Copy into w[0..15]
			int copy_index = 0;
			for (; copy_index < 16; copy_index++)
			{
				message_schedule[copy_index] = 0ul;
				for (int i = 0; i < word_bytes; i++)
				{
					message_schedule[copy_index] = message_schedule[copy_index] | ((ulong)input[(copy_index * word_bytes) + i] << (i * 8));
				}

				message_schedule[copy_index] = message_schedule[copy_index] & size_mask;
			}

			// temp vars
			ulong f, g;
			// work is equivalent to A, B, C, D
			// This copies work from a0, b0, c0, d0
			ulong[] work = new ulong[4];
			working_variables.CopyTo(work, 0);

			// Compression function main loop
			for (copy_index = 0; copy_index < round_count; copy_index++)
			{
				if (copy_index < 16)
				{
					f = ((work[1] & work[2]) | ((size_mask ^ work[1]) & work[3])) & size_mask;
					g = (ulong)copy_index;
				}
				else if (copy_index < 32)
				{
					f = ((work[3] & work[1]) | ((size_mask ^ work[3]) & work[2])) & size_mask;
					g = (ulong)(((5 * copy_index) + 1) % 16);
				}
				else if (copy_index < 48)
				{
					f = work[1] ^ work[2] ^ work[3];
					g = (ulong)(((3 * copy_index) + 5) % 16);
				}
				else
				{
					f = (work[2] ^ (work[1] | (size_mask ^ work[3]))) & size_mask;
					g = (ulong)(7 * copy_index % 16);
				}

				f = (f + work[0] + constants[copy_index] + message_schedule[g]) & size_mask;
				work[0] = work[3];
				work[3] = work[2];
				work[2] = work[1];
				work[1] = (work[1] + ((f << shifts[copy_index]) | (f >> word_size - shifts[copy_index]))) & size_mask;
			}

			for (copy_index = 0; copy_index < 4; copy_index++)
				working_variables[copy_index] = (working_variables[copy_index] + work[copy_index]) & size_mask;
		}

		// Finalization
		string output = "";

		for (int character_index = 0; character_index < output_segments; character_index++)
		{
			ulong value = working_variables[character_index];
			output += string.Format(output_format,
				((value & 0x000000FFul) << 0x18) |
				((value & 0x0000FF00ul) << 0x08) |
				((value & 0x00FF0000ul) >> 0x08) |
				((value & 0xFF000000ul) >> 0x18)
			);
		}

		return output;
	}

	public static string MD5_UTF8(string text)
	{
		int[] md5_shifts =
			{
			7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
			5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
			4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
			6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,
		};
		ulong[] md5_constants = {
			0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee, 0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
			0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be, 0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
			0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa, 0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
			0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed, 0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
			0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c, 0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
			0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05, 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
			0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039, 0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
			0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1, 0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391,
		};
		ulong[] md5_init = { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476 };

		return MD5_Core(ToUTF8(text.ToCharArray()), md5_init, md5_constants, md5_shifts, 0xFFFFFFFFul, 32, 64, 8, 64, "{0:x8}", 4);
	}

	// Sha2
	private static byte[] SHA2_Core(byte[] payload_bytes, ulong[] init, ulong[] constants, int[] sums, int[] sigmas, ulong size_mask, int word_size, int chunk_modulo, int appended_length, int round_count, int output_segments)
	{
		int word_bytes = word_size / 8;

		// Working variables h0->h7
		ulong[] working_variables = new ulong[8];
		init.CopyTo(working_variables, 0);

		byte[] input = new byte[chunk_modulo];
		ulong[] message_schedule = new ulong[round_count];

		// Each 64-byte/512-bit chunk
		// 64 bits/8 bytes are required at the end for the bit size
		for (int chunk_index = 0; chunk_index < payload_bytes.Length + appended_length + 1; chunk_index += chunk_modulo)
		{
			int chunk_size = Mathf.Min(chunk_modulo, payload_bytes.Length - chunk_index);
			int schedule_index = 0;

			// Buffer message
			for (; schedule_index < chunk_size; ++schedule_index)
				input[schedule_index] = payload_bytes[chunk_index + schedule_index];
			// Append a 1-bit if not an even chunk
			if (schedule_index < chunk_modulo && chunk_size >= 0)
				input[schedule_index++] = 0b10000000;
			// Pad with zeros until the end
			for (; schedule_index < chunk_modulo; ++schedule_index)
				input[schedule_index] = 0x00;
			// If the chunk is less than 56 bytes, this will be the final chunk containing the data size in bits
			if (chunk_size < chunk_modulo - appended_length)
			{
				ulong bit_size = (ulong)payload_bytes.Length * 8ul;
				input[chunk_modulo - 1] = Convert.ToByte((bit_size >> 0x00) & 0xFFul);
				input[chunk_modulo - 2] = Convert.ToByte((bit_size >> 0x08) & 0xFFul);
				input[chunk_modulo - 3] = Convert.ToByte((bit_size >> 0x10) & 0xFFul);
				input[chunk_modulo - 4] = Convert.ToByte((bit_size >> 0x18) & 0xFFul);
				input[chunk_modulo - 5] = Convert.ToByte((bit_size >> 0x20) & 0xFFul);
				input[chunk_modulo - 6] = Convert.ToByte((bit_size >> 0x28) & 0xFFul);
				input[chunk_modulo - 7] = Convert.ToByte((bit_size >> 0x30) & 0xFFul);
				input[chunk_modulo - 8] = Convert.ToByte((bit_size >> 0x38) & 0xFFul);
			}

			// Copy into w[0..15]
			int copy_index = 0;
			for (; copy_index < 16; copy_index++)
			{
				message_schedule[copy_index] = 0ul;
				for (int i = 0; i < word_bytes; i++)
				{
					message_schedule[copy_index] = (message_schedule[copy_index] << 8) | input[(copy_index * word_bytes) + i];
				}

				message_schedule[copy_index] = message_schedule[copy_index] & size_mask;
			}
			// Extend
			for (; copy_index < round_count; copy_index++)
			{
				ulong s0_read = message_schedule[copy_index - 15];
				ulong s1_read = message_schedule[copy_index - 2];

				message_schedule[copy_index] = (
					message_schedule[copy_index - 16] +
					(((s0_read >> sums[0]) | (s0_read << word_size - sums[0])) ^ ((s0_read >> sums[1]) | (s0_read << word_size - sums[1])) ^ (s0_read >> sums[2])) + // s0
					message_schedule[copy_index - 7] +
					(((s1_read >> sums[3]) | (s1_read << word_size - sums[3])) ^ ((s1_read >> sums[4]) | (s1_read << word_size - sums[4])) ^ (s1_read >> sums[5])) // s1
				) & size_mask;
			}

			// temp vars
			ulong temp1, temp2;
			// work is equivalent to a, b, c, d, e, f, g, h
			// This copies work from h0, h1, h2, h3, h4, h5, h6, h7
			ulong[] work = new ulong[8];
			working_variables.CopyTo(work, 0);

			// Compression function main loop
			for (copy_index = 0; copy_index < round_count; copy_index++)
			{
				ulong ep1 = ((work[4] >> sigmas[3]) | (work[4] << word_size - sigmas[3])) ^ ((work[4] >> sigmas[4]) | (work[4] << word_size - sigmas[4])) ^ ((work[4] >> sigmas[5]) | (work[4] << word_size - sigmas[5]));
				ulong ch = (work[4] & work[5]) ^ ((size_mask ^ work[4]) & work[6]);
				ulong ep0 = ((work[0] >> sigmas[0]) | (work[0] << word_size - sigmas[0])) ^ ((work[0] >> sigmas[1]) | (work[0] << word_size - sigmas[1])) ^ ((work[0] >> sigmas[2]) | (work[0] << word_size - sigmas[2]));
				ulong maj = (work[0] & work[1]) ^ (work[0] & work[2]) ^ (work[1] & work[2]);
				temp1 = work[7] + ep1 + ch + constants[copy_index] + message_schedule[copy_index];
				temp2 = ep0 + maj;
				work[7] = work[6];
				work[6] = work[5];
				work[5] = work[4];
				work[4] = (work[3] + temp1) & size_mask;
				work[3] = work[2];
				work[2] = work[1];
				work[1] = work[0];
				work[0] = (temp1 + temp2) & size_mask;
			}

			for (copy_index = 0; copy_index < 8; copy_index++)
				working_variables[copy_index] = (working_variables[copy_index] + work[copy_index]) & size_mask;
		}

		// Finalization
		byte[] output = new byte[output_segments * word_bytes];
		for (int i = 0; i < output_segments; i++)
		{
			for (int j = 0; j < word_bytes; j++)
			{
				output[(i * word_bytes) + j] = Convert.ToByte((working_variables[i] >> (word_bytes - j - 1) * 8) & 0xFFul);
			}
		}
		return output;
	}
	/* SHA224 */
	public static string SHA224_Bytes(byte[] data)
	{
		ulong[] sha224_init = {
			0xc1059ed8, 0x367cd507, 0x3070dd17, 0xf70e5939, 0xffc00b31, 0x68581511, 0x64f98fa7, 0xbefa4fa4,
		};

		ulong[] sha256_constants = {
			0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
			0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
			0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
			0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
			0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
			0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
			0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
			0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
		};

		int[] sha256_sums =
		{
			7, 18, 3,  // s0
			17, 19, 10,  // s1
		};

		int[] sha256_sigmas =
		{
			2, 13, 22,  // S0
			6, 11, 25,  // S1
		};

		return BytesToString(SHA2_Core(data, sha224_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 7));
	}

	public static string SHA224_UTF8(string text)
	{
		ulong[] sha224_init = {
			0xc1059ed8, 0x367cd507, 0x3070dd17, 0xf70e5939, 0xffc00b31, 0x68581511, 0x64f98fa7, 0xbefa4fa4,
		};

		ulong[] sha256_constants = {
			0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
			0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
			0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
			0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
			0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
			0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
			0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
			0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
		};

		int[] sha256_sums =
		{
			7, 18, 3,  // s0
			17, 19, 10,  // s1
		};

		int[] sha256_sigmas =
		{
			2, 13, 22,  // S0
			6, 11, 25,  // S1
		};

		return BytesToString(SHA2_Core(ToUTF8(text.ToCharArray()), sha224_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 7));
	}

	/* SHA256 */
	public static string SHA256_Bytes(byte[] data)
	{
		ulong[] sha256_constants = {
			0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
			0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
			0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
			0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
			0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
			0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
			0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
			0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
		};

		int[] sha256_sums =
		{
			7, 18, 3,  // s0
			17, 19, 10,  // s1
		};

		int[] sha256_sigmas =
		{
			2, 13, 22,  // S0
			6, 11, 25,  // S1
		};

		ulong[] sha256_init = {
			0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19,
		};

		return BytesToString(SHA2_Core(data, sha256_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 8));
	}

	public static string SHA256_UTF8(string text)
	{
		ulong[] sha256_constants = {
			0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
			0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
			0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
			0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
			0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
			0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
			0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
			0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
		};

		int[] sha256_sums =
		{
			7, 18, 3,  // s0
			17, 19, 10,  // s1
		};

		int[] sha256_sigmas =
		{
			2, 13, 22,  // S0
			6, 11, 25,  // S1
		};

		ulong[] sha256_init = {
			0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19,
		};

		return BytesToString(SHA2_Core(ToUTF8(text.ToCharArray()), sha256_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 8));
	}

	public static string HMAC_SHA256_Bytes(byte[] data, byte[] key)
	{

		ulong[] sha256_constants = {
			0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
			0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
			0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
			0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
			0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
			0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
			0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
			0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
		};

		int[] sha256_sums =
		{
			7, 18, 3,  // s0
			17, 19, 10,  // s1
		};

		int[] sha256_sigmas =
		{
			2, 13, 22,  // S0
			6, 11, 25,  // S1
		};

		ulong[] sha256_init = {
			0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19,
		};

		if (key.Length > 64)
		{
			key = SHA2_Core(key, sha256_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 8);
		}

		byte[] ipad = new byte[64];
		byte[] opad = new byte[64];

		for (int i = 0; i < 64; i++)
		{
			ipad[i] = 0x36;
			opad[i] = 0x5c;
		}

		for (int i = 0; i < key.Length; i++)
		{
			ipad[i] ^= key[i];
			opad[i] ^= key[i];
		}

		byte[] inner = new byte[ipad.Length + data.Length];

		ipad.CopyTo(inner, 0);
		data.CopyTo(inner, ipad.Length);

		byte[] innerHash = SHA2_Core(inner, sha256_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 8);

		byte[] outer = new byte[opad.Length + innerHash.Length];

		opad.CopyTo(outer, 0);
		innerHash.CopyTo(outer, opad.Length);

		return BytesToString(SHA2_Core(outer, sha256_init, sha256_constants, sha256_sums, sha256_sigmas, 0xFFFFFFFFul, 32, 64, 8, 64, 8));
	}

	public static string HMAC_SHA256_UTF8(string text, string key)
	{
		return HMAC_SHA256_Bytes(ToUTF8(text.ToCharArray()), ToUTF8(key.ToCharArray()));
	}

	/* SHA384 */
	public static string SHA384_Bytes(byte[] data)
	{
		ulong[] sha512_constants = {
			0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc, 0x3956c25bf348b538,
			0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118, 0xd807aa98a3030242, 0x12835b0145706fbe,
			0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235,
			0xc19bf174cf692694, 0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
			0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, 0x983e5152ee66dfab,
			0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4, 0xc6e00bf33da88fc2, 0xd5a79147930aa725,
			0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed,
			0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
			0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30, 0xd192e819d6ef5218,
			0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53,
			0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373,
			0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
			0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b, 0xca273eceea26619c,
			0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6,
			0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc,
			0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
		};

		int[] sha512_sums =
		{
			1, 8, 7,  // s0
			19, 61, 6,  // s1
		};

		int[] sha512_sigmas =
		{
			28, 34, 39,  // S0
			14, 18, 41,  // S1
		};

		ulong[] sha384_init = {
			0xcbbb9d5dc1059ed8, 0x629a292a367cd507, 0x9159015a3070dd17, 0x152fecd8f70e5939, 0x67332667ffc00b31,
			0x8eb44a8768581511, 0xdb0c2e0d64f98fa7, 0x47b5481dbefa4fa4,
		};

		return BytesToString(SHA2_Core(data, sha384_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 6));
	}

	public static string SHA384_UTF8(string text)
	{
		ulong[] sha512_constants = {
			0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc, 0x3956c25bf348b538,
			0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118, 0xd807aa98a3030242, 0x12835b0145706fbe,
			0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235,
			0xc19bf174cf692694, 0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
			0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, 0x983e5152ee66dfab,
			0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4, 0xc6e00bf33da88fc2, 0xd5a79147930aa725,
			0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed,
			0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
			0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30, 0xd192e819d6ef5218,
			0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53,
			0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373,
			0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
			0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b, 0xca273eceea26619c,
			0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6,
			0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc,
			0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
		};

		int[] sha512_sums =
		{
			1, 8, 7,  // s0
			19, 61, 6,  // s1
		};

		int[] sha512_sigmas =
		{
			28, 34, 39,  // S0
			14, 18, 41,  // S1
		};

		ulong[] sha384_init = {
			0xcbbb9d5dc1059ed8, 0x629a292a367cd507, 0x9159015a3070dd17, 0x152fecd8f70e5939, 0x67332667ffc00b31,
			0x8eb44a8768581511, 0xdb0c2e0d64f98fa7, 0x47b5481dbefa4fa4,
		};

		return BytesToString(SHA2_Core(ToUTF8(text.ToCharArray()), sha384_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 6));
	}

	/* SHA512 */
	public static string SHA512_Bytes(byte[] data)
	{

		ulong[] sha512_constants = {
			0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc, 0x3956c25bf348b538,
			0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118, 0xd807aa98a3030242, 0x12835b0145706fbe,
			0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235,
			0xc19bf174cf692694, 0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
			0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, 0x983e5152ee66dfab,
			0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4, 0xc6e00bf33da88fc2, 0xd5a79147930aa725,
			0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed,
			0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
			0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30, 0xd192e819d6ef5218,
			0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53,
			0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373,
			0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
			0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b, 0xca273eceea26619c,
			0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6,
			0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc,
			0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
		};

		int[] sha512_sums =
		{
			1, 8, 7,  // s0
			19, 61, 6,  // s1
		};

		int[] sha512_sigmas =
		{
			28, 34, 39,  // S0
			14, 18, 41,  // S1
		};

		ulong[] sha512_init = {
			0x6a09e667f3bcc908, 0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, 0xa54ff53a5f1d36f1, 0x510e527fade682d1,
			0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179,
		};

		return BytesToString(SHA2_Core(data, sha512_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 8));
	}

	public static string SHA512_UTF8(string text)
	{

		ulong[] sha512_constants = {
			0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc, 0x3956c25bf348b538,
			0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118, 0xd807aa98a3030242, 0x12835b0145706fbe,
			0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235,
			0xc19bf174cf692694, 0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
			0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, 0x983e5152ee66dfab,
			0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4, 0xc6e00bf33da88fc2, 0xd5a79147930aa725,
			0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed,
			0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
			0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30, 0xd192e819d6ef5218,
			0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53,
			0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373,
			0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
			0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b, 0xca273eceea26619c,
			0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6,
			0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc,
			0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
		};

		int[] sha512_sums =
		{
			1, 8, 7,  // s0
			19, 61, 6,  // s1
		};

		int[] sha512_sigmas =
		{
			28, 34, 39,  // S0
			14, 18, 41,  // S1
		};

		ulong[] sha512_init = {
			0x6a09e667f3bcc908, 0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, 0xa54ff53a5f1d36f1, 0x510e527fade682d1,
			0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179,
		};

		return BytesToString(SHA2_Core(ToUTF8(text.ToCharArray()), sha512_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 8));
	}

	public static string HMAC_SHA512_Bytes(byte[] data, byte[] key)
	{
		ulong[] sha512_constants = {
			0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc, 0x3956c25bf348b538,
			0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118, 0xd807aa98a3030242, 0x12835b0145706fbe,
			0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235,
			0xc19bf174cf692694, 0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
			0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, 0x983e5152ee66dfab,
			0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4, 0xc6e00bf33da88fc2, 0xd5a79147930aa725,
			0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed,
			0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
			0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30, 0xd192e819d6ef5218,
			0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53,
			0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373,
			0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
			0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b, 0xca273eceea26619c,
			0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6,
			0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc,
			0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
		};

		int[] sha512_sums =
		{
			1, 8, 7,  // s0
			19, 61, 6,  // s1
		};

		int[] sha512_sigmas =
		{
			28, 34, 39,  // S0
			14, 18, 41,  // S1
		};

		ulong[] sha512_init = {
			0x6a09e667f3bcc908, 0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, 0xa54ff53a5f1d36f1, 0x510e527fade682d1,
			0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179,
		};

		if (key.Length > 128)
		{
			key = SHA2_Core(key, sha512_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 8);
		}

		byte[] ipad = new byte[128];
		byte[] opad = new byte[128];

		for (int i = 0; i < 128; i++)
		{
			ipad[i] = 0x36;
			opad[i] = 0x5c;
		}

		for (int i = 0; i < key.Length; i++)
		{
			ipad[i] ^= key[i];
			opad[i] ^= key[i];
		}

		byte[] inner = new byte[ipad.Length + data.Length];

		ipad.CopyTo(inner, 0);
		data.CopyTo(inner, ipad.Length);

		byte[] innerHash = SHA2_Core(inner, sha512_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 8);

		byte[] outer = new byte[opad.Length + innerHash.Length];

		opad.CopyTo(outer, 0);
		innerHash.CopyTo(outer, opad.Length);

		return BytesToString(SHA2_Core(outer, sha512_init, sha512_constants, sha512_sums, sha512_sigmas, 0xFFFFFFFFFFFFFFFFul, 64, 128, 16, 80, 8));
	}

	public static string HMAC_SHA512_UTF8(string text, string key)
	{
		return HMAC_SHA512_Bytes(ToUTF8(text.ToCharArray()), ToUTF8(key.ToCharArray()));
	}
}


