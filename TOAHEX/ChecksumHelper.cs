using System;

namespace TOAHEX
{
    public static class ChecksumHelper
    {
        public static uint WordSum(byte[] data, int offset, int count)
        {
            uint total = 0;
            int end = offset + (count & ~3);
            for (int i = offset; i < end; i += 4)
            {
                total += BitConverter.ToUInt32(data, i);
            }
            return total;
        }

        public static void FixToaChecksum(byte[] data)
        {
            uint bodySum = WordSum(data, SaveOffsets.BODY_DATA_START, SaveOffsets.BODY_DATA_SIZE);
            WriteU32(data, SaveOffsets.BODY_CHECKSUM, bodySum);

            uint headSum = WordSum(data, SaveOffsets.HEAD_DATA_START, SaveOffsets.HEAD_DATA_SIZE);
            WriteU32(data, SaveOffsets.HEADER_CHECKSUM, headSum);
        }

        public static void FixToasysChecksum(byte[] data)
        {
            uint sum = WordSum(data, SaveOffsets.TOASYS_DATA_START, SaveOffsets.TOASYS_DATA_SIZE);
            WriteU32(data, SaveOffsets.TOASYS_CHECKSUM, sum);
        }

        public static bool VerifyToaChecksum(byte[] data)
        {
            uint storedHead = BitConverter.ToUInt32(data, SaveOffsets.HEADER_CHECKSUM);
            uint storedBody = BitConverter.ToUInt32(data, SaveOffsets.BODY_CHECKSUM);
            uint calcHead = WordSum(data, SaveOffsets.HEAD_DATA_START, SaveOffsets.HEAD_DATA_SIZE);
            uint calcBody = WordSum(data, SaveOffsets.BODY_DATA_START, SaveOffsets.BODY_DATA_SIZE);
            return storedHead == calcHead && storedBody == calcBody;
        }

        public static bool VerifyToasysChecksum(byte[] data)
        {
            uint stored = BitConverter.ToUInt32(data, SaveOffsets.TOASYS_CHECKSUM);
            uint calc = WordSum(data, SaveOffsets.TOASYS_DATA_START, SaveOffsets.TOASYS_DATA_SIZE);
            return stored == calc;
        }

        // ===== PS2 日版（算法与 3DS 相同：区域小端 u32 累加和，区域长度按 PS2 布局）=====

        public static void FixPs2ToaChecksum(byte[] data)
        {
            uint bodySum = WordSum(data, SaveOffsets.BODY_DATA_START, SaveOffsets.PS2_BODY_DATA_SIZE);
            WriteU32(data, SaveOffsets.BODY_CHECKSUM, bodySum);

            uint headSum = WordSum(data, SaveOffsets.HEAD_DATA_START, SaveOffsets.HEAD_DATA_SIZE);
            WriteU32(data, SaveOffsets.HEADER_CHECKSUM, headSum);
        }

        public static void FixPs2ToasysChecksum(byte[] data)
        {
            uint sum = WordSum(data, SaveOffsets.TOASYS_DATA_START, SaveOffsets.PS2_TOASYS_DATA_SIZE);
            WriteU32(data, SaveOffsets.TOASYS_CHECKSUM, sum);
        }

        public static bool VerifyPs2ToaChecksum(byte[] data)
        {
            uint storedHead = BitConverter.ToUInt32(data, SaveOffsets.HEADER_CHECKSUM);
            uint storedBody = BitConverter.ToUInt32(data, SaveOffsets.BODY_CHECKSUM);
            uint calcHead = WordSum(data, SaveOffsets.HEAD_DATA_START, SaveOffsets.HEAD_DATA_SIZE);
            uint calcBody = WordSum(data, SaveOffsets.BODY_DATA_START, SaveOffsets.PS2_BODY_DATA_SIZE);
            return storedHead == calcHead && storedBody == calcBody;
        }

        public static bool VerifyPs2ToasysChecksum(byte[] data)
        {
            uint stored = BitConverter.ToUInt32(data, SaveOffsets.TOASYS_CHECKSUM);
            uint calc = WordSum(data, SaveOffsets.TOASYS_DATA_START, SaveOffsets.PS2_TOASYS_DATA_SIZE);
            return stored == calc;
        }

        private static void WriteU32(byte[] data, int offset, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            System.Buffer.BlockCopy(bytes, 0, data, offset, 4);
        }
    }
}
