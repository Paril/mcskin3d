using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ILboolean = System.Boolean;
using ILint = System.Int32;
using ILuint = System.UInt32;
using ILclampf = System.Single;
using ILdouble = System.Double;
using ILsizei = System.UInt32;

namespace DevCIL
{
	public static class IL
	{
		//  Matches OpenGL's right now.
		//! Data formats \link Formats Formats\endlink
		public enum ColorFormats
		{
			ColourIndex = 0x1900,
			ColorIndex = 0x1900,
			Alpha = 0x1906,
			RGB = 0x1907,
			RGBA = 0x1908,
			BGR = 0x80E0,
			BGRA = 0x80E1,
			Luminance = 0x1909,
			LuminanceAlpha = 0x190A,
		}

		//! Data types \link Types Types\endlink
		public enum Types
		{
			Byte = 0x1400,
			UnsignedByte = 0x1401,
			Short = 0x1402,
			SignedShort = 0x1403,
			Int = 0x1404,
			UnsignedInt = 0x1405,
			Float = 0x1406,
			Single = 0x1406,
			Double = 0x140A,
			Half = 0x140B,
		};

		public const int IL_VENDOR   = 0x1F00;
		public const int IL_LOAD_EXT = 0x1F01;
		public const int IL_SAVE_EXT = 0x1F02;

		// Attribute Bits
		[Flags]
		public enum AttribBits
		{
			Origin = 0x00000001,
			File = 0x00000002,
			Palette = 0x00000004,
			Format = 0x00000008,
			Type = 0x00000010,
			Compress = 0x00000020,
			LoadFail = 0x00000040,
			FormatSpecific = 0x00000080,
			All = 0x000FFFFF,
		}

		public enum PaletteType
		{
			// Palette types
			None = 0x0400,
			RGB24 = 0x0401,
			RGB32 = 0x0402,
			RGBA32 = 0x0403,
			BGR24 = 0x0404,
			BGR32 = 0x0405,
			BGRA32 = 0x0406,
		}

		// Image types
		public enum ImageType
		{
			Unknown = 0x0000,
			BMP = 0x0420,  //!< Microsoft Windows Bitmap - .bmp extension
			CUT = 0x0421,  //!< Dr. Halo - .cut extension
			DOOM = 0x0422,  //!< DooM walls - no specific extension
			DOOM_FLAT = 0x0423,  //!< DooM flats - no specific extension
			ICO = 0x0424,  //!< Microsoft Windows Icons and Cursors - .ico and .cur extensions
			JPG = 0x0425,  //!< JPEG - .jpg, .jpe and .jpeg extensions
			JFIF = 0x0425,  //!<
			ILBM = 0x0426,  //!< Amiga IFF (FORM ILBM) - .iff, .ilbm, .lbm extensions
			PCD = 0x0427,  //!< Kodak PhotoCD - .pcd extension
			PCX = 0x0428,  //!< ZSoft PCX - .pcx extension
			PIC = 0x0429,  //!< PIC - .pic extension
			PNG = 0x042A,  //!< Portable Network Graphics - .png extension
			PNM = 0x042B,  //!< Portable Any Map - .pbm, .pgm, .ppm and .pnm extensions
			SGI = 0x042C,  //!< Silicon Graphics - .sgi, .bw, .rgb and .rgba extensions
			TGA = 0x042D,  //!< TrueVision Targa File - .tga, .vda, .icb and .vst extensions
			TIF = 0x042E,  //!< Tagged Image File Format - .tif and .tiff extensions
			CHEAD = 0x042F,  //!< C-Style Header - .h extension
			RAW = 0x0430,  //!< Raw Image Data - any extension
			MDL = 0x0431,  //!< Half-Life Model Texture - .mdl extension
			WAL = 0x0432,  //!< Quake 2 Texture - .wal extension
			LIF = 0x0434,  //!< Homeworld Texture - .lif extension
			MNG = 0x0435,  //!< Multiple-image Network Graphics - .mng extension
			JNG = 0x0435,  //!< 
			GIF = 0x0436,  //!< Graphics Interchange Format - .gif extension
			DDS = 0x0437,  //!< DirectDraw Surface - .dds extension
			DCX = 0x0438,  //!< ZSoft Multi-PCX - .dcx extension
			PSD = 0x0439,  //!< Adobe PhotoShop - .psd extension
			EXIF = 0x043A,  //!< 
			PSP = 0x043B,  //!< PaintShop Pro - .psp extension
			PIX = 0x043C,  //!< PIX - .pix extension
			PXR = 0x043D,  //!< Pixar - .pxr extension
			XPM = 0x043E,  //!< X Pixel Map - .xpm extension
			HDR = 0x043F,  //!< Radiance High Dynamic Range - .hdr extension
			ICNS = 0x0440,  //!< Macintosh Icon - .icns extension
			JP2 = 0x0441,  //!< Jpeg 2000 - .jp2 extension
			EXR = 0x0442,  //!< OpenEXR - .exr extension
			WDP = 0x0443,  //!< Microsoft HD Photo - .wdp and .hdp extension
			VTF = 0x0444,  //!< Valve Texture Format - .vtf extension
			WBMP = 0x0445,  //!< Wireless Bitmap - .wbmp extension
			SUN = 0x0446,  //!< Sun Raster - .sun, .ras, .rs, .im1, .im8, .im24 and .im32 extensions
			IFF = 0x0447,  //!< Interchange File Format - .iff extension
			TPL = 0x0448,  //!< Gamecube Texture - .tpl extension
			FITS = 0x0449,  //!< Flexible Image Transport System - .fit and .fits extensions
			DICOM = 0x044A,  //!< Digital Imaging and Communications in Medicine (DICOM) - .dcm and .dicom extensions
			IWI = 0x044B,  //!< Call of Duty Infinity Ward Image - .iwi extension
			BLP = 0x044C,  //!< Blizzard Texture Format - .blp extension
			FTX = 0x044D,  //!< Heavy Metal: FAKK2 Texture - .ftx extension
			ROT = 0x044E,  //!< Homeworld 2 - Relic Texture - .rot extension
			TEXTURE = 0x044F,  //!< Medieval II: Total War Texture - .texture extension
			DPX = 0x0450,  //!< Digital Picture Exchange - .dpx extension
			UTX = 0x0451,  //!< Unreal (and Unreal Tournament) Texture - .utx extension
			MP3 = 0x0452,  //!< MPEG-1 Audio Layer 3 - .mp3 extension
			JASC_PAL = 0x0475,  //!< PaintShop Pro Palette
		};

		// Error Types
		public enum ErrorCode
		{
			NoError = 0x0000,
			InvalidEnum = 0x0501,
			OutOfMemory = 0x0502,
			FormatNotSupported = 0x0503,
			InternalError = 0x0504,
			InvalidValue = 0x0505,
			IllegalOperation = 0x0506,
			IllegalFileValue = 0x0507,
			InvalidFileHeader = 0x0508,
			InvalidParameter = 0x0509,
			CouldntOpenFile = 0x050A,
			InvalidExtension = 0x050B,
			FileAlreadyExists = 0x050C,
			OutFormatSame = 0x050D,
			StackOverflow = 0x050E,
			StackUnderflow = 0x050F,
			InvalidConversion = 0x0510,
			BadDimensions = 0x0511,
			FileReadError = 0x0512,  // 05/12/2002: Addition by Sam.
			FileWriteError = 0x0512,

			LibGIFError = 0x05E1,
			LibJPEGError = 0x05E2,
			LibPNGError = 0x05E3,
			LibTIFFError = 0x05E4,
			LibMNGError = 0x05E5,
			LibJP2Error = 0x05E6,
			LibEXRError = 0x05E7,
			Unknown = 0x05FF,
		}


		// Origin Definitions
		public enum Origin
		{
			LowerLeft = 0x0601,
			UpperLeft = 0x0602,
		};

		public enum Parameters
		{
			// Format and Type Mode Definitions
			FormatSet = 0x0610,
			FormatMode = 0x0611,
			TypeSet = 0x0612,
			TypeMode = 0x0613,


			// File definitions
			FileOverwrite = 0x0620,
			FileMode = 0x0621,


			// Palette definitions
			ConvertPalette = 0x0630,


			// Load fail definitions
			DefaultOnFail = 0x0632,


			// Key colour and alpha definitions
			UseKeyColour = 0x0635,
			UseKeyColor = 0x0635,
			BlitBlend = 0x0636,


			// Interlace definitions
			SaveInterlaced = 0x0639,
			InterlaceMode = 0x063A,


			// Quantization definitions
			QuantizationMode = 0x0640,
			WUQuant = 0x0641,
			NEUQuant = 0x0642,
			NEUQuantSample = 0x0643,
			MaxQuantIndexS = 0x0644, //XIX : ILint : Maximum number of colors to reduce to, default of 256. and has a range of 2-256
			MaxQuantIndices = 0x0644, // Redefined, since the above is = misspelled,

			VersionNumber = 0x0DE2,
			ImageWidth = 0x0DE4,
			ImageHeight = 0x0DE5,
			ImageDepth = 0x0DE6,
			ImageSizeOfData = 0x0DE7,
			ImageBPP = 0x0DE8,
			ImageBytesPerPixel = 0x0DE8,
			ImageBitsPerPixel = 0x0DE9,
			ImageFormat = 0x0DEA,
			ImageType = 0x0DEB,
			PaletteType = 0x0DEC,
			PaletteSize = 0x0DED,
			PaletteBPP = 0x0DEE,
			PaletteNumColumns = 0x0DEF,
			PaletteBaseType = 0x0DF0,
			NumFaces = 0x0DE1,
			NumImages = 0x0DF1,
			NumMipmaps = 0x0DF2,
			NumLayers = 0x0DF3,
			ActiveImage = 0x0DF4,
			ActiveMipmape = 0x0DF5,
			ActiveLayer = 0x0DF6,
			ActiveFace = 0x0E00,
			CurImage = 0x0DF7,
			ImageDuration = 0x0DF8,
			ImagePlaneSide = 0x0DF9,
			ImageBPC = 0x0DFA,
			ImageOffsetX = 0x0DFB,
			ImageOffsetY = 0x0DFC,
			ImageCubeFlags = 0x0DFD,
			ImageOrigin = 0x0DFE,
			ImageChannels = 0x0DFF,

			nVidiaCompress = 0x0670,
			SquishCompress = 0x0671,

			OriginSet = 0x0600,
			OriginMode = 0x0603,
		}

		// Hints
		public enum HintTarget
		{
			MemSpeedHint = 0x0665,
			CompressionHint = 0x0668,
		}

		public enum HintMode
		{
			Fastest = 0x0660,
			LessMem = 0x0661,
			DontCare = 0x0662,
			UseCompression = 0x0666,
			NoCompression = 0x0667,
		}

		// Subimage types
		public enum SubImageTypes
		{
			Next = 0x0680,
			Mipmap = 0x0681,
			Layer = 0x0682,
		}

		// Compression definitions
		public enum CompressMode
		{
			Mode = 0x0700,
			None = 0x0701,
			RLE = 0x0702,
			LZO = 0x0703,
			ZLIB = 0x0704,
		}


		// File format-specific values
		public enum FormatSpecific
		{
			TGACreateStamp = 0x0710,
			JPGQuality = 0x0711,
			PNGInterlace = 0x0712,
			TGARLE = 0x0713,
			BMPRLE = 0x0714,
			SGIRLE = 0x0715,
			TGAIDString = 0x0717,
			TGAAuthNameString = 0x0718,
			TGAAuthCommentString = 0x0719,
			PNGAuthNameString = 0x071A,
			PNGTitleString = 0x071B,
			PNGDescriptionString = 0x071C,
			TIFDescriptionString = 0x071D,
			TIFHostComputerString = 0x071E,
			TIFDocumentNameString = 0x071F,
			TIFAuthNameString = 0x0720,
			JPGSaveormat = 0x0721,
			CHEADHeaderString = 0x0722,
			PCDPicNnum = 0x0723,
			PNGAlphaIndex = 0x0724, //XIX : ILint : the color in the palette at this index value (0-255) is considered transparent, -1 for no trasparent color
			JPGProgressive = 0x0725,
			VTFComp = 0x0726,
		};


		// DXTC definitions
		public enum DXTCFormats
		{
			DXTCFormat = 0x0705,
			DXT1 = 0x0706,
			DXT2 = 0x0707,
			DXT3 = 0x0708,
			DXT4 = 0x0709,
			DXT5 = 0x070A,
			DXTNoComp = 0x070B,
			KeepDXTCData = 0x070C,
			DXTCDataFormat = 0x070D,
			DXCT_3DC = 0x070E,
			RXGB = 0x070F,
			ATI1N = 0x0710,
			DXT1A = 0x0711,  // Normally the same as IL_DXT1, except for nVidia Texture Tools.
		}

		// Environment map definitions
		public enum EnvMapFlags
		{
			CubemapPositiveX = 0x00000400,
			CubemapNegativeX = 0x00000800,
			CubemapPositiveY = 0x00001000,
			CubemapNegativeY = 0x00002000,
			CubemapPositiveZ = 0x00004000,
			CubemapNegativeZ = 0x00008000,
			Spheremap = 0x00010000,
		}

		// Callback functions for file reading
		public delegate void fCloseRProc(IntPtr handle);
		public delegate ILboolean fEofProc(IntPtr handle);
		public delegate ILint fGetcProc(IntPtr handle);
		public delegate IntPtr fOpenRProc([MarshalAs(UnmanagedType.LPStr)] string fileName);
		public delegate ILint fReadProc(IntPtr buffer, ILuint size, ILuint count, IntPtr handle);
		public delegate ILint fSeekRProc(IntPtr handle, ILint offset, ILint origin);
		public delegate ILint fTellRProc(IntPtr handle);

		// Callback functions for file writing
		public delegate void fCloseWProc(IntPtr handle);
		public delegate IntPtr fOpenWProc([MarshalAs(UnmanagedType.LPStr)] string fileName);
		public delegate ILint fPutcProc(byte c, IntPtr handle);
		public delegate ILint fSeekWProc(IntPtr handle, ILint offset, ILint origin);
		public delegate ILint fTellWProc(IntPtr handle);
		public delegate ILint fWriteProc(IntPtr buffer, ILuint size, ILuint count, IntPtr handle);

		// Callback functions for allocation and deallocation
		public delegate IntPtr mAlloc(ILsizei size);
		public delegate void mFree(IntPtr ptr);

		// Registered format procedures
		public delegate ImageType IL_LOADPROC([MarshalAs(UnmanagedType.LPStr)] string fileName);
		public delegate ImageType IL_SAVEPROC([MarshalAs(UnmanagedType.LPStr)] string fileName);


		public const string DEVIL = "DevIL.dll";

		[DllImport(DEVIL)]
		public static extern ILboolean ilActiveFace(uint Number);

		[DllImport(DEVIL)]
		public static extern ILboolean ilActiveImage(uint Number);

		[DllImport(DEVIL)]
		public static extern ILboolean ilActiveLayer(ILuint Number);

		[DllImport(DEVIL)]
		public static extern ILboolean ilActiveMipmap(ILuint Number);

		[DllImport(DEVIL)]
		public static extern ILboolean ilApplyPal([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilApplyProfile([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilApplyProfile([MarshalAs(UnmanagedType.LPStr)] string InProfile, [MarshalAs(UnmanagedType.LPStr)] string OutProfile);

		[DllImport(DEVIL)]
		public static extern void ilBindImage(ILuint Image);

		[DllImport(DEVIL)]
		public static extern ILboolean ilBlit(ILuint Source, ILint DestX, ILint DestY, ILint DestZ, ILint SrcX, ILint SrcY, ILint SrcZ, ILuint Width, ILuint Height, ILuint Depth);

		[DllImport(DEVIL)]
		public static extern ILboolean ilClampNTSC();

		[DllImport(DEVIL, EntryPoint = "ilClearColour")]
		public static extern void ilClearColor(ILclampf Red, ILclampf Green, ILclampf Blue, ILclampf Alpha);

		[DllImport(DEVIL)]
		public static extern ILboolean ilClearImage();

		[DllImport(DEVIL)]
		public static extern ILuint ilCloneCurImage();

		[DllImport(DEVIL)]
		public static extern IntPtr ilCompressDXT(IntPtr Data, ILuint Width, ILuint Height, ILuint Depth, DXTCFormats DXTCFormat, [Out] ILint DXTCSize);

		[DllImport(DEVIL)]
		public static extern ILboolean ilCompressFunc(CompressMode Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilConvertImage(ColorFormats DestFormat, Types DestType);

		[DllImport(DEVIL)]
		public static extern ILboolean ilConvertPal(ColorFormats DestFormat);

		[DllImport(DEVIL)]
		public static extern ILboolean ilCopyImage(ILuint Src);

		[DllImport(DEVIL)]
		public static extern ILuint ilCopyPixels(ILuint XOff, ILuint YOff, ILuint ZOff, ILuint Width, ILuint Height, ILuint Depth, ILint Format, ILint Type, IntPtr Data);

		[DllImport(DEVIL)]
		public static extern ILuint ilCreateSubImage(SubImageTypes Type, ILuint Num);

		[DllImport(DEVIL)]
		public static extern ILboolean ilDefaultImage();

		[DllImport(DEVIL)]
		public static extern void ilDeleteImage(ILuint Num);

		[DllImport(DEVIL)]
		public static extern void ilDeleteImages(ILuint Num, [MarshalAs(UnmanagedType.LPArray)] ILuint[] Images);

		[DllImport(DEVIL)]
		public static extern ImageType ilDetermineType([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ImageType ilDetermineTypeF(IntPtr File);

		[DllImport(DEVIL)]
		public static extern ImageType ilDetermineTypeL(IntPtr Lump, ILuint Size);

		[DllImport(DEVIL)]
		public static extern ILboolean ilDisable(Parameters Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilDxtcDataToImage();

		[DllImport(DEVIL)]
		public static extern ILboolean ilDxtcDataToSurface();

		[DllImport(DEVIL)]
		public static extern ILboolean ilEnable(Parameters Mode);

		[DllImport(DEVIL)]
		public static extern void ilFlipSurfaceDxtcData();

		[DllImport(DEVIL)]
		public static extern ILboolean ilFormatFunc(ColorFormats Mode);

		[DllImport(DEVIL)]
		public static extern void ilGenImages(ILuint Num, [Out] ILuint[] Images);

		[DllImport(DEVIL)]
		public static extern ILuint ilGenImage();

		[DllImport(DEVIL)]
		public static extern IntPtr ilGetAlpha(Types Type);

		[DllImport(DEVIL)]
		public static extern ILboolean ilGetBoolean(Parameters Mode);

		[DllImport(DEVIL)]
		public static extern void ilGetBooleanv(Parameters Mode, [Out] ILboolean[] Param);

		[DllImport(DEVIL)]
		public static extern IntPtr ilGetData();

		[DllImport(DEVIL)]
		public static extern ILuint ilGetDXTCData([Out] byte[] Buffer, ILuint BufferSize, DXTCFormats DXTCFormat);

		[DllImport(DEVIL)]
		public static extern ErrorCode ilGetError();

		[DllImport(DEVIL)]
		public static extern ILint ilGetInteger(Parameters Mode);

		[DllImport(DEVIL)]
		public static extern void ilGetIntegerv(Parameters Mode, [Out] ILint[] Param);

		[DllImport(DEVIL)]
		public static extern ILuint ilGetLumpPos();

		[DllImport(DEVIL)]
		public static extern IntPtr GetPalette();

		[DllImport(DEVIL)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern string ilGetString(Parameters StringName);

		[DllImport(DEVIL)]
		public static extern void ilHint(HintTarget Target, HintMode Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilInvertSurfaceDxtcDataAlpha();

		[DllImport(DEVIL)]
		public static extern void ilInit();

		[DllImport(DEVIL)]
		public static extern ILboolean ilImageToDxtcData(DXTCFormats Format);

		[DllImport(DEVIL)]
		public static extern ILboolean ilIsDisabled(Parameters Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilIsEnabled(Parameters Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilIsImage(ILuint Image);

		[DllImport(DEVIL)]
		public static extern ILboolean ilIsValid(ImageType Type, [MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilIsValidF(ImageType Type, IntPtr File);

		[DllImport(DEVIL)]
		public static extern ILint ilIsValidL(IntPtr Lump, ILuint Size);

		[DllImport(DEVIL)]
		public static extern void ilKeyColour(ILclampf Red, ILclampf Green, ILclampf Blue, ILclampf Alpha);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoad(ImageType Type, [MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadF(ImageType Type, IntPtr File);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadImage([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadL(IntPtr Lump, ILuint Size);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadPal([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern void ilModAlpha(ILdouble AlphaValue);

		[DllImport(DEVIL)]
		public static extern ILboolean ilOriginFunc(Origin Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilOverlayImage(ILuint Source, ILint XCoord, ILint YCoord, ILint ZCoord);

		[DllImport(DEVIL)]
		public static extern void ilPopAttrib();

		[DllImport(DEVIL)]
		public static extern void ilPushAttrib(ILuint Bits);

		[DllImport(DEVIL)]
		public static extern void ilRegisterFormat(ImageType Format);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRegisterLoad([MarshalAs(UnmanagedType.LPStr)] string Ext, IL_LOADPROC Load);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRegisterMipNum(uint Num);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRegisterNumFaces(uint Num);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRegisterNumImages(uint Num);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRegisterOrigin(Origin Origin);



		[DllImport(DEVIL)]
		public static extern void ilRegisterPal(IntPtr Pal, uint Size, PaletteType Type);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRegisterSave([MarshalAs(UnmanagedType.LPStr)] string Ext, IL_SAVEPROC Save);

		[DllImport(DEVIL)]
		public static extern void ilRegisterType(Types Type);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRemoveLoad([MarshalAs(UnmanagedType.LPStr)] string Ext);

		[DllImport(DEVIL)]
		public static extern ILboolean ilRemoveSave([MarshalAs(UnmanagedType.LPStr)] string Ext);

		[DllImport(DEVIL)]
		[Obsolete]
		public static extern void ilResetMemory();

		[DllImport(DEVIL)]
		public static extern void ilResetRead();

		[DllImport(DEVIL)]
		public static extern void ilResetWrite();

		[DllImport(DEVIL)]
		public static extern ILboolean ilSave(ImageType Type, [MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILuint ilSaveF(ImageType Type, IntPtr File);

		[DllImport(DEVIL)]
		public static extern ILboolean ilSaveImage([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILuint ilSaveL(ImageType Type, IntPtr Lump, ILuint Size);

		[DllImport(DEVIL)]
		public static extern ILboolean ilSavePal([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilSetAlpha(ILdouble AlphaValue);

		[DllImport(DEVIL)]
		public static extern ILboolean ilSetData(IntPtr Data);

		[DllImport(DEVIL)]
		public static extern ILboolean ilSetDuration(ILuint Duration);

		[DllImport(DEVIL)]
		public static extern void ilSetInteger(Parameters Mode, ILint Param);

		[DllImport(DEVIL)]
		public static extern void ilSetMemory(mAlloc alloc, mFree free);

		[DllImport(DEVIL)]
		public static extern void ilSetPixels(ILint XOff, ILint YOff, ILint ZOff, ILuint Width, ILuint Height, ILuint Depth, ColorFormats Format, Types Type, IntPtr Data);

		[DllImport(DEVIL)]
		public static extern void ilSetRead(fOpenRProc open, fCloseRProc close, fEofProc eof, fGetcProc getc, fReadProc read, fSeekRProc seek, fTellRProc tell);

		[DllImport(DEVIL)]
		public static extern void ilSetString(Parameters Mode, [MarshalAs(UnmanagedType.LPStr)] string String);

		[DllImport(DEVIL)]
		public static extern void ilSetWrite(fOpenWProc open, fCloseWProc close, fPutcProc putc, fSeekWProc seek, fTellWProc tell, fWriteProc write);

		[DllImport(DEVIL)]
		public static extern void ilShutDown();

		[DllImport(DEVIL)]
		public static extern ILboolean ilSurfaceToDxtcData(DXTCFormats Format);

		[DllImport(DEVIL)]
		public static extern ILboolean ilTexImage(ILuint Width, ILuint Height, ILuint Depth, byte NumChannels, ColorFormats Format, Types Type, IntPtr Data);

		[DllImport(DEVIL)]
		public static extern ILboolean ilTexImageDxtc(ILint w, ILint h, ILint d, DXTCFormats DxtFormat, IntPtr data);

		[DllImport(DEVIL)]
		public static extern ImageType ilTypeFromExt([MarshalAs(UnmanagedType.LPStr)] string FileName);

		[DllImport(DEVIL)]
		public static extern ILboolean ilTypeFunc(Types Mode);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadData([MarshalAs(UnmanagedType.LPStr)] string FileName, ILuint Width, ILuint Height, ILuint Depth, byte Bpp);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadDataF(IntPtr File, ILuint Width, ILuint Height, ILuint Depth, byte Bpp);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadDataL(IntPtr Lump, ILuint Size, ILuint Width, ILuint Height, ILuint Depth, byte Bpp);

		[DllImport(DEVIL)]
		public static extern ILboolean ilLoadDataL([In] byte[] Lump, ILuint Size, ILuint Width, ILuint Height, ILuint Depth, byte Bpp);

		[DllImport(DEVIL)]
		public static extern ILboolean ilSaveData([MarshalAs(UnmanagedType.LPStr)] string FileName);

		/*
			ILAPI ILboolean ILAPIENTRY ilActiveFace(ILuint Number);
			ILAPI ILboolean ILAPIENTRY ilActiveImage(ILuint Number);
			ILAPI ILboolean ILAPIENTRY ilActiveLayer(ILuint Number);
			ILAPI ILboolean ILAPIENTRY ilActiveMipmap(ILuint Number);
			ILAPI ILboolean ILAPIENTRY ilApplyPal(ILconst_string FileName);
			ILAPI ILboolean ILAPIENTRY ilApplyProfile(ILstring InProfile, ILstring OutProfile);
			ILAPI void		ILAPIENTRY ilBindImage(ILuint Image);
			ILAPI ILboolean ILAPIENTRY ilBlit(ILuint Source, ILint DestX, ILint DestY, ILint DestZ, ILuint SrcX, ILuint SrcY, ILuint SrcZ, ILuint Width, ILuint Height, ILuint Depth);
			ILAPI ILboolean ILAPIENTRY ilClampNTSC(void);
			ILAPI void		ILAPIENTRY ilClearColour(ILclampf Red, ILclampf Green, ILclampf Blue, ILclampf Alpha);
			ILAPI ILboolean ILAPIENTRY ilClearImage(void);
			ILAPI ILuint    ILAPIENTRY ilCloneCurImage(void);
			ILAPI ILubyte*	ILAPIENTRY ilCompressDXT(ILubyte *Data, ILuint Width, ILuint Height, ILuint Depth, ILenum DXTCFormat, ILuint *DXTCSize);
			ILAPI ILboolean ILAPIENTRY ilCompressFunc(ILenum Mode);
			ILAPI ILboolean ILAPIENTRY ilConvertImage(ILenum DestFormat, ILenum DestType);
			ILAPI ILboolean ILAPIENTRY ilConvertPal(ILenum DestFormat);
			ILAPI ILboolean ILAPIENTRY ilCopyImage(ILuint Src);
			ILAPI ILuint    ILAPIENTRY ilCopyPixels(ILuint XOff, ILuint YOff, ILuint ZOff, ILuint Width, ILuint Height, ILuint Depth, ILenum Format, ILenum Type, void *Data);
			ILAPI ILuint    ILAPIENTRY ilCreateSubImage(ILenum Type, ILuint Num);
			ILAPI ILboolean ILAPIENTRY ilDefaultImage(void);
			ILAPI void		ILAPIENTRY ilDeleteImage(const ILuint Num);
			ILAPI void      ILAPIENTRY ilDeleteImages(ILsizei Num, const ILuint *Images);
			ILAPI ILenum	ILAPIENTRY ilDetermineType(ILconst_string FileName);
			ILAPI ILenum	ILAPIENTRY ilDetermineTypeF(ILHANDLE File);
			ILAPI ILenum	ILAPIENTRY ilDetermineTypeL(const void *Lump, ILuint Size);
			ILAPI ILboolean ILAPIENTRY ilDisable(ILenum Mode);
			ILAPI ILboolean ILAPIENTRY ilDxtcDataToImage(void);
			ILAPI ILboolean ILAPIENTRY ilDxtcDataToSurface(void);
			ILAPI ILboolean ILAPIENTRY ilEnable(ILenum Mode);
			ILAPI void		ILAPIENTRY ilFlipSurfaceDxtcData(void);
			ILAPI ILboolean ILAPIENTRY ilFormatFunc(ILenum Mode);
			ILAPI void	    ILAPIENTRY ilGenImages(ILsizei Num, ILuint *Images);
			ILAPI ILuint	ILAPIENTRY ilGenImage(void);
			ILAPI ILubyte*  ILAPIENTRY ilGetAlpha(ILenum Type);
			ILAPI ILboolean ILAPIENTRY ilGetBoolean(ILenum Mode);
			ILAPI void      ILAPIENTRY ilGetBooleanv(ILenum Mode, ILboolean *Param);
			ILAPI ILubyte*  ILAPIENTRY ilGetData(void);
			*/

		/*
		ILAPI ILuint    ILAPIENTRY ilGetDXTCData(void *Buffer, ILuint BufferSize, ILenum DXTCFormat);
		ILAPI ILenum    ILAPIENTRY ilGetError(void);
		ILAPI ILint     ILAPIENTRY ilGetInteger(ILenum Mode);
		ILAPI void      ILAPIENTRY ilGetIntegerv(ILenum Mode, ILint *Param);
		ILAPI ILuint    ILAPIENTRY ilGetLumpPos(void);
		ILAPI ILubyte*  ILAPIENTRY ilGetPalette(void);
		ILAPI ILconst_string  ILAPIENTRY ilGetString(ILenum StringName);
		ILAPI void      ILAPIENTRY ilHint(ILenum Target, ILenum Mode);
		ILAPI ILboolean	ILAPIENTRY ilInvertSurfaceDxtcDataAlpha(void);
		ILAPI void      ILAPIENTRY ilInit(void);
		ILAPI ILboolean ILAPIENTRY ilImageToDxtcData(ILenum Format);
		ILAPI ILboolean ILAPIENTRY ilIsDisabled(ILenum Mode);
		ILAPI ILboolean ILAPIENTRY ilIsEnabled(ILenum Mode);
		ILAPI ILboolean ILAPIENTRY ilIsImage(ILuint Image);
		ILAPI ILboolean ILAPIENTRY ilIsValid(ILenum Type, ILconst_string FileName);
		ILAPI ILboolean ILAPIENTRY ilIsValidF(ILenum Type, ILHANDLE File);
		ILAPI ILboolean ILAPIENTRY ilIsValidL(ILenum Type, void *Lump, ILuint Size);
		ILAPI void      ILAPIENTRY ilKeyColour(ILclampf Red, ILclampf Green, ILclampf Blue, ILclampf Alpha);
		ILAPI ILboolean ILAPIENTRY ilLoad(ILenum Type, ILconst_string FileName);
		ILAPI ILboolean ILAPIENTRY ilLoadF(ILenum Type, ILHANDLE File);
		ILAPI ILboolean ILAPIENTRY ilLoadImage(ILconst_string FileName);
		ILAPI ILboolean ILAPIENTRY ilLoadL(ILenum Type, const void *Lump, ILuint Size);
		ILAPI ILboolean ILAPIENTRY ilLoadPal(ILconst_string FileName);
		ILAPI void      ILAPIENTRY ilModAlpha(ILdouble AlphaValue);
		ILAPI ILboolean ILAPIENTRY ilOriginFunc(ILenum Mode);
		ILAPI ILboolean ILAPIENTRY ilOverlayImage(ILuint Source, ILint XCoord, ILint YCoord, ILint ZCoord);
		ILAPI void      ILAPIENTRY ilPopAttrib(void);
		ILAPI void      ILAPIENTRY ilPushAttrib(ILuint Bits);
		ILAPI void      ILAPIENTRY ilRegisterFormat(ILenum Format);
		ILAPI ILboolean ILAPIENTRY ilRegisterLoad(ILconst_string Ext, IL_LOADPROC Load);
		ILAPI ILboolean ILAPIENTRY ilRegisterMipNum(ILuint Num);
		ILAPI ILboolean ILAPIENTRY ilRegisterNumFaces(ILuint Num);
		ILAPI ILboolean ILAPIENTRY ilRegisterNumImages(ILuint Num);
		ILAPI void      ILAPIENTRY ilRegisterOrigin(ILenum Origin);
		*/
		/*
			ILAPI void      ILAPIENTRY ilRegisterPal(void *Pal, ILuint Size, ILenum Type);
			ILAPI ILboolean ILAPIENTRY ilRegisterSave(ILconst_string Ext, IL_SAVEPROC Save);
			ILAPI void      ILAPIENTRY ilRegisterType(ILenum Type);
			ILAPI ILboolean ILAPIENTRY ilRemoveLoad(ILconst_string Ext);
			ILAPI ILboolean ILAPIENTRY ilRemoveSave(ILconst_string Ext);
			ILAPI void      ILAPIENTRY ilResetMemory(void); // Deprecated
			ILAPI void      ILAPIENTRY ilResetRead(void);
			ILAPI void      ILAPIENTRY ilResetWrite(void);
			ILAPI ILboolean ILAPIENTRY ilSave(ILenum Type, ILconst_string FileName);
			ILAPI ILuint    ILAPIENTRY ilSaveF(ILenum Type, ILHANDLE File);
			ILAPI ILboolean ILAPIENTRY ilSaveImage(ILconst_string FileName);
			ILAPI ILuint    ILAPIENTRY ilSaveL(ILenum Type, void *Lump, ILuint Size);
			ILAPI ILboolean ILAPIENTRY ilSavePal(ILconst_string FileName);
			ILAPI ILboolean ILAPIENTRY ilSetAlpha(ILdouble AlphaValue);
			ILAPI ILboolean ILAPIENTRY ilSetData(void *Data);
			ILAPI ILboolean ILAPIENTRY ilSetDuration(ILuint Duration);
			ILAPI void      ILAPIENTRY ilSetInteger(ILenum Mode, ILint Param);
			ILAPI void      ILAPIENTRY ilSetMemory(mAlloc, mFree);
			ILAPI void      ILAPIENTRY ilSetPixels(ILint XOff, ILint YOff, ILint ZOff, ILuint Width, ILuint Height, ILuint Depth, ILenum Format, ILenum Type, void *Data);
			ILAPI void      ILAPIENTRY ilSetRead(fOpenRProc, fCloseRProc, fEofProc, fGetcProc, fReadProc, fSeekRProc, fTellRProc);
			ILAPI void      ILAPIENTRY ilSetString(ILenum Mode, const char *String);
			ILAPI void      ILAPIENTRY ilSetWrite(fOpenWProc, fCloseWProc, fPutcProc, fSeekWProc, fTellWProc, fWriteProc);
			ILAPI void      ILAPIENTRY ilShutDown(void);
			ILAPI ILboolean ILAPIENTRY ilSurfaceToDxtcData(ILenum Format);
			ILAPI ILboolean ILAPIENTRY ilTexImage(ILuint Width, ILuint Height, ILuint Depth, ILubyte NumChannels, ILenum Format, ILenum Type, void *Data);
			ILAPI ILboolean ILAPIENTRY ilTexImageDxtc(ILint w, ILint h, ILint d, ILenum DxtFormat, const ILubyte* data);
			ILAPI ILenum    ILAPIENTRY ilTypeFromExt(ILconst_string FileName);
			ILAPI ILboolean ILAPIENTRY ilTypeFunc(ILenum Mode);
			ILAPI ILboolean ILAPIENTRY ilLoadData(ILconst_string FileName, ILuint Width, ILuint Height, ILuint Depth, ILubyte Bpp);
			ILAPI ILboolean ILAPIENTRY ilLoadDataF(ILHANDLE File, ILuint Width, ILuint Height, ILuint Depth, ILubyte Bpp);
			ILAPI ILboolean ILAPIENTRY ilLoadDataL(void *Lump, ILuint Size, ILuint Width, ILuint Height, ILuint Depth, ILubyte Bpp);
			ILAPI ILboolean ILAPIENTRY ilSaveData(ILconst_string FileName);
		*/
	}
}
