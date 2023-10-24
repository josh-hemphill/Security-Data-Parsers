using System.Collections;

namespace BinaryUtils;

internal class Binary {
	public static BitArray ByteToBitArr( byte[] x ) {
		return new( Array.ConvertAll( x, Convert.ToBoolean ) );
	}

	public static bool[] Bit5ToBoolArr( byte x ) {
		BitArray bitArr = new( new byte[] { x } );
		bool[] bits = new bool[bitArr.Length];
		bitArr.CopyTo( bits, 0 );
		bits = bits.Reverse().ToArray();
		return bits.Skip( bits.Length - 5 ).ToArray();
	}
	public static bool[] ByteToBoolArr( byte[] x ) {
		BitArray bitArr = new BitArray( x.ToArray() ).Not();
		bool[] bits = new bool[bitArr.Length];
		bitArr.CopyTo( bits, 0 );
		return bits;
	}
	public static bool[] ByteToBoolArr( ReadOnlyMemory<byte> x ) {
		BitArray bitArr = new BitArray( x.ToArray() ).Not();
		bool[] bits = new bool[bitArr.Length];
		bitArr.CopyTo( bits, 0 );
		return bits;
	}
	public static bool Left5bitsMatch( bool[] sourceArr, byte matchingBits, byte sourceOffset ) {
		return GetLeft5AsByte( sourceArr, sourceOffset ) == matchingBits;
	}

	public static bool[] GetLeft5Bits( bool[] source, int offset ) {
		return new bool[] {
				source[0 + offset],
				source[1 + offset],
				source[2 + offset],
				source[3 + offset],
				source[4 + offset]
			};
	}

	public static byte GetLeft5AsByte( bool[] source, int offset ) {
		BitArray bitArr = new( GetLeft5Bits( source, offset ) );
		byte[] bits = new byte[bitArr.Length];
		bitArr.CopyTo( bits, 0 );
		return bits[0];
	}

	public static byte ToByte( bool[] bits ) {
		return bits.Length > 8
		  ? throw new ArgumentException( "BoolArray is too large to cast to a byte" )
		  : BoolArrayReverseToByte( bits );
	}
	public static byte MathWithinByte( int mathResult ) {
		return mathResult >= 255
			 ? throw new InvalidDataException( "Integer is larger than a byte, out of bounds" )
			 : (byte)mathResult;
	}
	public static byte BoolArrayReverseToByteOld( bool[] ba ) {
		/* ulong n = 0; */
		byte n = 0;
		for (int i = 0; i <= 8 && i < ba.Length; i++) {
			n <<= 1;
			if (ba[i]) {
				n |= 0b1;
			}
			/* n <<= 1;
			if (i < ba.Count && ba.Get(i)) n |= 1; */
		}
		return n;
	}
	private const byte tByte = 1;
	private const byte fByte = 0;
	public static byte BoolArrayReverseToByte( bool[] ba ) {
		byte d1 = ba[0] ? tByte : fByte;
		byte d2 = (byte)((ba[1] ? tByte : fByte) << 1);
		byte d3 = (byte)((ba[2] ? tByte : fByte) << 2);
		byte d4 = (byte)((ba[3] ? tByte : fByte) << 3);
		byte result = (byte)(d1 + d2 + d3 + d4);
		return result;
	}
	public static bool[] ByteToBoolArr( byte value ) {
		bool[] values = new bool[8];
		for (byte i = 0, j = 1; i < 8; i++, j <<= 1) {
			values[i] = (value & j) != 0;
		}
		return values.Reverse().ToArray();
	}
	public static bool[] GetBytesAsBoolArr( byte[] b ) {
		return b.SelectMany( ByteToBoolArr ).ToArray();
	}


	public static byte[] HexStrToByteArr( string hex ) =>
		hex.Select( x => byte.Parse( new ReadOnlySpan<char>( x ), System.Globalization.NumberStyles.HexNumber ) ).ToArray();

	public static byte BitSetStrToByte( string bits ) => ToByte( new BitArray( new bool[] {
			bits[0] == '1',
			bits[1] == '1',
			bits[2] == '1',
			bits[3] == '1',
			bits[4] == '1',
			bits[5] == '1',
			bits[6] == '1',
			bits[7] == '1'
		} ) );
	public static byte[] BitStrToByteArr( string bits ) {
		int numOfBytes = bits.Length / 8;
		byte[] bytes = new byte[numOfBytes];
		for (int i = 0; i < numOfBytes; ++i) {
			bytes[i] = Convert.ToByte( bits.Substring( 8 * i, 8 ), 2 );
		}
		return bytes;
	}
	public static byte[] IntStrToByteArr( string digits ) => digits.Select( v => byte.Parse( new ReadOnlySpan<char>( v ) ) ).ToArray();
	public static byte ToByte( BitArray bits ) {
		if (bits.Count > 8) {
			throw new ArgumentException( "BitArray is too large to cast to a byte" );
		}
		byte[] bytes = new byte[1];
		bits.CopyTo( bytes, 0 );
		return bytes[0];
	}
}
