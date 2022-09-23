using System;
using System.Text;

namespace WC2.Unpacker
{
    class PackageCipher
    {
        // Modified Salsa20 :)

        static UInt32[] m_State;
        static readonly Int32 dwRounds = 20;
        static readonly Int32 dwBlockSize = 64;

        static Byte[] m_Constants = Encoding.ASCII.GetBytes("nd 32-byte kexpa");
        static Byte[] m_Key = Encoding.ASCII.GetBytes("0123456789abcdefghijklmnopqrstu\0");

        private static UInt32 ToUInt32(Byte[] lpBuffer, Int32 dwOffset)
        {
            unchecked
            {
                return (UInt32)(((lpBuffer[dwOffset] |
                                (lpBuffer[dwOffset + 1] << 8)) |
                                (lpBuffer[dwOffset + 2] << 16)) |
                                (lpBuffer[dwOffset + 3] << 24));
            }
        }

        private static void ToBytes(UInt32 lpSrcBuffer, Byte[] lpDstBuffer, Int32 dwOffset)
        {
            unchecked
            {
                lpDstBuffer[dwOffset] = (Byte)lpSrcBuffer;
                lpDstBuffer[dwOffset + 1] = (Byte)(lpSrcBuffer >> 8);
                lpDstBuffer[dwOffset + 2] = (Byte)(lpSrcBuffer >> 16);
                lpDstBuffer[dwOffset + 3] = (Byte)(lpSrcBuffer >> 24);
            }
        }

        private static UInt32 ROL(UInt32 dwValue, Int32 dwShift)
        {
            return (dwValue << dwShift) | (dwValue >> (dwBlockSize / 2 - dwShift));
        }

        private static UInt32 ROR(UInt32 dwValue, Int32 dwShift)
        {
            return (dwValue >> dwShift) | (dwValue << (dwBlockSize / 2 - dwShift));
        }

        private static UInt32 ADD(UInt32 dwValueA, UInt32 dwValueB)
        {
            return unchecked(dwValueA + dwValueB);
        }

        private static void iInitState(Byte[] m_Key, Byte[] m_Vector, UInt32 dwCounter)
        {
            m_State = new UInt32[16];

            m_State[0] = ToUInt32(m_Constants, 0);
            m_State[1] = ToUInt32(m_Constants, 4);
            m_State[2] = ToUInt32(m_Constants, 8);
            m_State[3] = ToUInt32(m_Constants, 12);

            m_State[4] = ToUInt32(m_Key, 0);
            m_State[5] = ToUInt32(m_Key, 4);
            m_State[6] = ToUInt32(m_Key, 8);
            m_State[7] = ToUInt32(m_Key, 12);

            m_State[8] = ToUInt32(m_Vector, 0);
            m_State[9] = ToUInt32(m_Vector, 4);

            m_State[10] = dwCounter;
            m_State[11] = 0;

            m_State[12] = ToUInt32(m_Key, 16);
            m_State[13] = ToUInt32(m_Key, 20);
            m_State[14] = ToUInt32(m_Key, 24);
            m_State[15] = ToUInt32(m_Key, 28);
        }

        private static void iBlockTransform(Byte[] lpBlockData, UInt32[] m_State)
        {
            UInt32[] m_BlockState = (UInt32[])m_State.Clone();

            for (Int32 i = 0; i < dwRounds / 2; i++)
            {
                m_BlockState[7] ^= ROL(ADD(m_BlockState[13], m_BlockState[3]), 7);
                m_BlockState[10] ^= ROL(ADD(m_BlockState[7], m_BlockState[3]), 9);
                m_BlockState[13] ^= ROL(ADD(m_BlockState[10], m_BlockState[7]), 13);
                m_BlockState[3] ^= ROR(ADD(m_BlockState[10], m_BlockState[13]), 14);

                m_BlockState[11] ^= ROL(ADD(m_BlockState[4], m_BlockState[0]), 7);
                m_BlockState[14] ^= ROL(ADD(m_BlockState[0], m_BlockState[11]), 9);
                m_BlockState[4] ^= ROL(ADD(m_BlockState[14], m_BlockState[11]), 13);
                m_BlockState[0] ^= ROR(ADD(m_BlockState[14], m_BlockState[4]), 14);

                m_BlockState[15] ^= ROL(ADD(m_BlockState[1], m_BlockState[8]), 7);
                m_BlockState[5] ^= ROL(ADD(m_BlockState[1], m_BlockState[15]), 9);
                m_BlockState[8] ^= ROL(ADD(m_BlockState[15], m_BlockState[5]), 13);
                m_BlockState[1] ^= ROR(ADD(m_BlockState[8], m_BlockState[5]), 14);

                m_BlockState[6] ^= ROL(ADD(m_BlockState[2], m_BlockState[12]), 7);
                m_BlockState[9] ^= ROL(ADD(m_BlockState[2], m_BlockState[6]), 9);
                m_BlockState[12] ^= ROL(ADD(m_BlockState[9], m_BlockState[6]), 13);
                m_BlockState[2] ^= ROR(ADD(m_BlockState[9], m_BlockState[12]), 14);

                m_BlockState[4] ^= ROL(ADD(m_BlockState[6], m_BlockState[3]), 7);
                m_BlockState[5] ^= ROL(ADD(m_BlockState[4], m_BlockState[3]), 9);
                m_BlockState[6] ^= ROL(ADD(m_BlockState[5], m_BlockState[4]), 13);
                m_BlockState[3] ^= ROR(ADD(m_BlockState[6], m_BlockState[5]), 14);

                m_BlockState[8] ^= ROL(ADD(m_BlockState[7], m_BlockState[0]), 7);
                m_BlockState[9] ^= ROL(ADD(m_BlockState[8], m_BlockState[0]), 9);
                m_BlockState[7] ^= ROL(ADD(m_BlockState[9], m_BlockState[8]), 13);
                m_BlockState[0] ^= ROR(ADD(m_BlockState[9], m_BlockState[7]), 14);

                m_BlockState[12] ^= ROL(ADD(m_BlockState[1], m_BlockState[11]), 7);
                m_BlockState[10] ^= ROL(ADD(m_BlockState[1], m_BlockState[12]), 9);
                m_BlockState[11] ^= ROL(ADD(m_BlockState[10], m_BlockState[12]), 13);
                m_BlockState[1] ^= ROR(ADD(m_BlockState[10], m_BlockState[11]), 14);

                m_BlockState[13] ^= ROL(ADD(m_BlockState[2], m_BlockState[15]), 7);
                m_BlockState[14] ^= ROL(ADD(m_BlockState[2], m_BlockState[13]), 9);
                m_BlockState[15] ^= ROL(ADD(m_BlockState[14], m_BlockState[13]), 13);
                m_BlockState[2] ^= ROR(ADD(m_BlockState[15], m_BlockState[14]), 14);
            }

            ToBytes(ADD(m_State[3], m_BlockState[3]), lpBlockData, 0);
            ToBytes(ADD(m_State[4], m_BlockState[4]), lpBlockData, 4);
            ToBytes(ADD(m_State[5], m_BlockState[5]), lpBlockData, 8);
            ToBytes(ADD(m_State[6], m_BlockState[6]), lpBlockData, 12);
            ToBytes(ADD(m_State[7], m_BlockState[7]), lpBlockData, 16);
            ToBytes(ADD(m_State[0], m_BlockState[0]), lpBlockData, 20);
            ToBytes(ADD(m_State[8], m_BlockState[8]), lpBlockData, 24);
            ToBytes(ADD(m_State[9], m_BlockState[9]), lpBlockData, 28);
            ToBytes(ADD(m_State[10], m_BlockState[10]), lpBlockData, 32);
            ToBytes(ADD(m_State[11], m_BlockState[11]), lpBlockData, 36);
            ToBytes(ADD(m_State[1], m_BlockState[1]), lpBlockData, 40);
            ToBytes(ADD(m_State[12], m_BlockState[12]), lpBlockData, 44);
            ToBytes(ADD(m_State[13], m_BlockState[13]), lpBlockData, 48);
            ToBytes(ADD(m_State[14], m_BlockState[14]), lpBlockData, 52);
            ToBytes(ADD(m_State[15], m_BlockState[15]), lpBlockData, 56);
            ToBytes(ADD(m_State[2], m_BlockState[2]), lpBlockData, 60);
        }

        public static Byte[] iDecryptData(Byte[] lpBuffer, Byte[] m_Vector)
        {
            Byte[] lpResult = new Byte[lpBuffer.Length];
            Byte[] lpBlockData = new Byte[dwBlockSize];
            Int32 dwOffset = 0;
            UInt32 dwCounter = 0;

            while (dwOffset < lpBuffer.Length)
            {
                iInitState(m_Key, m_Vector, dwCounter);
                iBlockTransform(lpBlockData, m_State);

                m_State[8] = m_State[8] + 1;
                if (m_State[8] == 0)
                {
                    m_State[9] = m_State[9] + 1;
                }

                Int32 dwCurrentBlockSize = dwBlockSize > lpBuffer.Length - dwOffset ? lpBuffer.Length - dwOffset : dwBlockSize;

                for (Int32 i = 0; i < dwCurrentBlockSize; i++)
                {
                    lpResult[dwOffset + i] = (Byte)(lpBuffer[dwOffset + i] ^ lpBlockData[i]);
                }

                dwOffset += dwBlockSize;
                dwCounter++;
            }

            return lpResult;
        }
    }
}
