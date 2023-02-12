// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Gamejam/OceanDisplace"
{
	Properties
	{
		_SeaColor("Sea Color", Color) = (0,0.3254902,0.4039216,1)
		_Displace_3D("Displace_3D", 3D) = "white" {}
		_Normals_3D("Normals_3D", 3D) = "white" {}
		_Distort("Distort", Range( 0 , 0.2)) = 0.08510161
		_NormalPower("Normal Power", Range( 0 , 1)) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.9
		_Depth("Depth", Range( 0 , 1)) = 1
		_Tiling("Tiling", Float) = 1
		_WaveSpeed("Wave Speed", Float) = 0.4
		_EdgePower("Edge Power", Range( 0 , 1)) = 0.3
		_WaveHeight("Wave Height", Range( 0 , 1)) = 1
		_CausticsScale("Caustics Scale", Float) = 5
		[HDR]_CausticsColour("Caustics Colour", Color) = (2,2,2,0)
		_WaveMask("WaveMask", 2D) = "white" {}
		_MaskStrength("Mask Strength", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float eyeDepth;
			float4 screenPos;
		};

		uniform sampler3D _Displace_3D;
		uniform float _Tiling;
		uniform float _WaveSpeed;
		uniform float _WaveHeight;
		uniform sampler2D _WaveMask;
		uniform float _MaskStrength;
		uniform sampler3D _Normals_3D;
		uniform float _NormalPower;
		uniform float4 _SeaColor;
		uniform float _CausticsScale;
		uniform float _Distort;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float4 _CausticsColour;
		uniform float _Depth;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _EdgePower;
		uniform float _Smoothness;


		float2 voronoihash108( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi108( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash108( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.707 * sqrt(dot( r, r ));
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


		float2 UnStereo( float2 UV )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
			UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
			#endif
			return UV;
		}


		float3 InvertDepthDir72_g107( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime74 = _Time.y * _WaveSpeed;
			float3 appendResult75 = (float3(( ase_worldPos.x * _Tiling ) , ( ase_worldPos.z * _Tiling ) , mulTime74));
			float3 _3duvs127 = appendResult75;
			float4 tex3DNode61 = tex3Dlod( _Displace_3D, float4( _3duvs127, 0.0) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 appendResult280 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.z));
			float _cheekySmoother338 = saturate( ( tex2Dlod( _WaveMask, float4( (appendResult280*0.043 + 0.5), 0, 0.0) ).r + (1.0 + (_MaskStrength - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float4 vertexOffset133 = ( tex3DNode61 * float4( ( ( float3(0,1,0) * (0.0 + (_WaveHeight - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) ) * _cheekySmoother338 ) , 0.0 ) );
			v.vertex.xyz += vertexOffset133.rgb;
			v.vertex.w = 1;
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float mulTime74 = _Time.y * _WaveSpeed;
			float3 appendResult75 = (float3(( ase_worldPos.x * _Tiling ) , ( ase_worldPos.z * _Tiling ) , mulTime74));
			float3 _3duvs127 = appendResult75;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float lerpResult392 = lerp( 0.0 , _NormalPower , ase_worldNormal.y);
			float3 normals123 = UnpackScaleNormal( tex3D( _Normals_3D, _3duvs127 ), lerpResult392 );
			o.Normal = normals123;
			float4 tex3DNode61 = tex3D( _Displace_3D, _3duvs127 );
			float4 _displace305 = tex3DNode61;
			float4 lerpResult307 = lerp( _SeaColor , ( _SeaColor + 0.1 ) , _displace305);
			float4 Albedo137 = lerpResult307;
			o.Albedo = Albedo137.rgb;
			float _Time130 = mulTime74;
			float time108 = ( _Time130 * 8.0 );
			float2 voronoiSmoothId0 = 0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth28_g102 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float2 temp_output_20_0_g102 = ( (normals123).xy * ( _Distort / max( i.eyeDepth , 0.1 ) ) * saturate( ( eyeDepth28_g102 - i.eyeDepth ) ) );
			float eyeDepth2_g102 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ( float4( temp_output_20_0_g102, 0.0 , 0.0 ) + ase_screenPosNorm ).xy ));
			float2 temp_output_32_0_g102 = (( float4( ( temp_output_20_0_g102 * saturate( ( eyeDepth2_g102 - i.eyeDepth ) ) ), 0.0 , 0.0 ) + ase_screenPosNorm )).xy;
			float2 temp_output_1_0_g102 = ( ( floor( ( temp_output_32_0_g102 * (_CameraDepthTexture_TexelSize).zw ) ) + 0.5 ) * (_CameraDepthTexture_TexelSize).xy );
			float2 temp_output_150_38 = temp_output_1_0_g102;
			float2 _refractUV168 = temp_output_150_38;
			float2 temp_output_73_0_g107 = _refractUV168;
			float2 UV22_g108 = float4( temp_output_73_0_g107, 0.0 , 0.0 ).xy;
			float2 localUnStereo22_g108 = UnStereo( UV22_g108 );
			float2 break64_g107 = localUnStereo22_g108;
			float clampDepth69_g107 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, float4( temp_output_73_0_g107, 0.0 , 0.0 ).xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g107 = ( 1.0 - clampDepth69_g107 );
			#else
				float staticSwitch38_g107 = clampDepth69_g107;
			#endif
			float3 appendResult39_g107 = (float3(break64_g107.x , break64_g107.y , staticSwitch38_g107));
			float4 appendResult42_g107 = (float4((appendResult39_g107*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g107 = mul( unity_CameraInvProjection, appendResult42_g107 );
			float3 In72_g107 = ( (temp_output_43_0_g107).xyz / (temp_output_43_0_g107).w );
			float3 localInvertDepthDir72_g107 = InvertDepthDir72_g107( In72_g107 );
			float4 appendResult49_g107 = (float4(localInvertDepthDir72_g107 , 1.0));
			float2 coords108 = (mul( unity_CameraToWorld, appendResult49_g107 )).xz * _CausticsScale;
			float2 id108 = 0;
			float2 uv108 = 0;
			float voroi108 = voronoi108( coords108, time108, id108, uv108, 0, voronoiSmoothId0 );
			float smoothstepResult273 = smoothstep( 0.38 , 1.25 , voroi108);
			float eyeDepth238 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, float4( _refractUV168, 0.0 , 0.0 ).xy ));
			float temp_output_239_0 = ( eyeDepth238 - i.eyeDepth );
			float _depthMask259 = ( 1.0 - saturate( ( temp_output_239_0 + (-1.0 + (_Depth - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) ) );
			float4 screenColor149 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,temp_output_150_38);
			float4 _distortion189 = screenColor149;
			float _edgeFoam295 = ( 1.0 - saturate( ( temp_output_239_0 * 12.0 ) ) );
			float4 emission121 = ( ( ( ( saturate( smoothstepResult273 ) * _CausticsColour ) * _depthMask259 ) + saturate( ( _distortion189 * _depthMask259 ) ) ) + ( _edgeFoam295 * _EdgePower ) );
			o.Emission = emission121.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
0;1080;1280;699;4230.115;2467.25;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;77;-3726.676,-1729.457;Inherit;False;1083.396;512.6696;Comment;9;127;75;73;74;72;70;71;130;343;3d_TextureUVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-3645.677,-1484.815;Inherit;False;Property;_Tiling;Tiling;7;0;Create;True;0;0;0;False;0;False;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;343;-3661.034,-1682.624;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;71;-3421.677,-1344.457;Inherit;False;Property;_WaveSpeed;Wave Speed;8;0;Create;True;0;0;0;False;0;False;0.4;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-3289.677,-1507.457;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-3283.677,-1675.457;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;74;-3245.677,-1340.457;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;395;-2075.532,-1652.859;Inherit;False;677.3872;403.3722;Comment;3;392;391;106;SideFix;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;75;-3055.677,-1603.457;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;127;-2888.676,-1606.82;Inherit;False;_3duvs;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-1950.614,-1599.652;Inherit;False;Property;_NormalPower;Normal Power;4;0;Create;True;0;0;0;False;0;False;1;0.678;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;391;-1888.542,-1440.287;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;140;-1241.929,-1728.598;Inherit;False;1011.688;304.9631;Comment;3;123;66;331;Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;392;-1663.547,-1513.171;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;331;-1091.778,-1670.662;Inherit;False;127;_3duvs;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;66;-865.4077,-1672.597;Inherit;True;Property;_Normals_3D;Normals_3D;2;0;Create;True;0;0;0;False;0;False;-1;15730400ed428da42a95ec30431ce5d4;15730400ed428da42a95ec30431ce5d4;True;0;False;white;LockedToTexture3D;True;Object;-1;Auto;Texture3D;8;0;SAMPLER3D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-490.7915,-1649.108;Inherit;False;normals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;151;-4662.679,-1003.585;Inherit;False;1302.432;449.3139;Comment;6;189;148;149;168;152;150;Refraction + UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-4618.741,-772.4777;Inherit;False;Property;_Distort;Distort;3;0;Create;True;0;0;0;False;0;False;0.08510161;0.2;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-4610.243,-946.8492;Inherit;False;123;normals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;150;-4315.004,-928.4248;Inherit;False;Sai_DepthMaskedRefraction;-1;;102;a63c900487a193f41a51cf8b7e7f977f;2,40,0,103,0;2;35;FLOAT3;0,0,0;False;37;FLOAT;0.02;False;1;FLOAT2;38
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-3888.979,-719.2836;Inherit;False;_refractUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;244;-3951.183,-2593.947;Inherit;False;1724.984;573.2555;Comment;15;334;259;296;253;297;238;255;299;298;239;295;235;251;240;256;DepthMasks;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;235;-3910.736,-2540.544;Inherit;False;168;_refractUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;291;-4211.086,-173.5038;Inherit;False;1766.387;542.8701;Comment;11;336;288;333;289;280;290;282;279;337;338;340;Cheeky Wave Smoother;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;251;-3730.196,-2293.465;Inherit;False;Property;_Depth;Depth;6;0;Create;True;0;0;0;False;0;False;1;0.658;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;240;-3760.161,-2429.487;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;238;-3712.688,-2539.492;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;60;-1968.951,-2483.267;Inherit;False;1919.79;438.3458;Comment;11;273;145;117;200;108;17;131;144;143;341;399;Caustics;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;279;-4161.085,-123.5037;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-2966.677,-1346.457;Inherit;False;_Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;334;-3419.126,-2248.128;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;239;-3450.438,-2484.329;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;-1907.575,-2434.325;Inherit;False;168;_refractUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;280;-3959.979,-89.38065;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;399;-1698.962,-2427.968;Inherit;False;Reconstruct World Position From Depth 2;-1;;107;4f04fb07708c6c44ba16506cff07981e;0;1;73;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;282;-4004.524,53.77597;Inherit;False;Constant;_ProjectionScale;ProjectionScale;12;0;Create;True;0;0;0;False;0;False;0.043;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;290;-4004.484,140.7239;Inherit;False;Constant;_ProjectionOffset;ProjectionOffset;13;0;Create;True;0;0;0;False;0;False;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;255;-3217.707,-2265.362;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;-1463.07,-2344.943;Inherit;False;130;_Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-3218.662,-2408.425;Inherit;False;Constant;_Float3;Float 3;14;0;Create;True;0;0;0;False;0;False;12;12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;333;-3616.818,194.698;Inherit;False;Property;_MaskStrength;Mask Strength;14;0;Create;True;0;0;0;False;0;False;1;0.543;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-1280.088,-2230.279;Inherit;False;Property;_CausticsScale;Caustics Scale;11;0;Create;True;0;0;0;False;0;False;5;14.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;17;-1291.887,-2437.194;Inherit;False;True;False;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;256;-3069.247,-2263.274;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1275.677,-2342.984;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;289;-3690.443,9.53789;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;340;-3318.271,189.4939;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-3043.4,-2485.103;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;253;-2909.6,-2265.313;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;288;-3456.293,-20.373;Inherit;True;Property;_WaveMask;WaveMask;13;0;Create;True;0;0;0;False;0;False;-1;9e4afcd6d6fda81418ccf68f6cded108;9e4afcd6d6fda81418ccf68f6cded108;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VoronoiNode;108;-1016.423,-2436.766;Inherit;True;0;1;1;3;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;5;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.ScreenColorNode;149;-3868.942,-934.9869;Inherit;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;396;156.1215,-2456.116;Inherit;False;1767.147;515.429;Comment;12;263;304;247;301;248;264;262;266;303;265;302;121;Depth_Merged;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;135;-1920.05,-855.0711;Inherit;False;1515.38;700.3642;Comment;11;129;133;93;142;61;79;141;277;305;335;339;Displace;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;189;-3574.268,-937.0414;Inherit;False;_distortion;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;298;-2866.194,-2479.848;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;273;-789.8936,-2422.108;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.38;False;2;FLOAT;1.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-2495.558,-2273.792;Inherit;False;_depthMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;336;-3082.524,1.871921;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;263;520.2737,-2229.927;Inherit;False;189;_distortion;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;299;-2689.092,-2479.141;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;341;-737.241,-2252.204;Inherit;False;Property;_CausticsColour;Caustics Colour;12;1;[HDR];Create;True;0;0;0;False;0;False;2,2,2,0;1.207273,1.741101,1.620283,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;265;518.7541,-2103.694;Inherit;False;259;_depthMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;117;-552.4708,-2353.657;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;129;-1848.038,-770.4926;Inherit;False;127;_3duvs;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;337;-2907.362,6.73198;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-1888.74,-358.4025;Inherit;False;Property;_WaveHeight;Wave Height;10;0;Create;True;0;0;0;False;0;False;1;0.31;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;295;-2490.498,-2511.697;Inherit;False;_edgeFoam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;61;-1631.354,-799.071;Inherit;True;Property;_Displace_3D;Displace_3D;1;0;Create;True;0;0;0;False;0;False;-1;ede26a347e1c6634d9d6c87e30361481;ede26a347e1c6634d9d6c87e30361481;True;0;False;white;LockedToTexture3D;False;Object;-1;Auto;Texture3D;8;0;SAMPLER3D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-376.1655,-2283.904;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;141;-1607.331,-524.4096;Inherit;False;Constant;_Vector0;Vector 0;9;0;Create;True;0;0;0;False;0;False;0,1,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;248;202.7264,-2235.463;Inherit;False;259;_depthMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;264;741.9996,-2200.24;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;139;-3020.286,-852.8118;Inherit;False;929.8838;385.0968;Comment;6;306;307;310;67;312;137;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;338;-2716.411,5.78193;Inherit;False;_cheekySmoother;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;335;-1611.758,-352.7395;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;302;1144.767,-2167.331;Inherit;False;295;_edgeFoam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;424.3591,-2351.701;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;312;-2969.724,-623.6198;Inherit;False;Constant;_Float4;Float 4;14;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;266;911.4647,-2207.191;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;339;-1265.586,-281.1995;Inherit;False;338;_cheekySmoother;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-1416.988,-437.5934;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;67;-2992.201,-805.8253;Inherit;False;Property;_SeaColor;Sea Color;0;0;Create;True;0;0;0;False;0;False;0,0.3254902,0.4039216,1;0,0.3709992,0.4039216,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;304;1053.222,-2066.687;Inherit;False;Property;_EdgePower;Edge Power;9;0;Create;True;0;0;0;False;0;False;0.3;0.627;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;305;-1275.095,-655.9173;Inherit;False;_displace;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;262;1067.203,-2373.361;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;277;-992.5174,-591.9963;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;303;1348.221,-2138.687;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;306;-2709.911,-593.3343;Inherit;False;305;_displace;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;310;-2731.65,-717.2911;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.142978;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;301;1496.638,-2373.642;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;307;-2454.896,-794.9055;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-826.8288,-800.2112;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;1703.329,-2361.079;Inherit;False;emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-2279.269,-797.3871;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;-616.6078,-803.1741;Inherit;False;vertexOffset;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;132;-123.8674,-200.7202;Inherit;False;123;normals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-120.0694,-315.9911;Inherit;False;137;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;134;-143.3189,179.8605;Inherit;False;133;vertexOffset;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-227.4595,45.258;Inherit;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;0;False;0;False;0.9;0.474;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;190;-126.5638,-86.2793;Inherit;False;121;emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;400;179.1833,-181.2293;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Gamejam/OceanDisplace;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;73;0;343;3
WireConnection;73;1;70;0
WireConnection;72;0;343;1
WireConnection;72;1;70;0
WireConnection;74;0;71;0
WireConnection;75;0;72;0
WireConnection;75;1;73;0
WireConnection;75;2;74;0
WireConnection;127;0;75;0
WireConnection;392;1;106;0
WireConnection;392;2;391;2
WireConnection;66;1;331;0
WireConnection;66;5;392;0
WireConnection;123;0;66;0
WireConnection;150;35;152;0
WireConnection;150;37;148;0
WireConnection;168;0;150;38
WireConnection;238;0;235;0
WireConnection;130;0;74;0
WireConnection;334;0;251;0
WireConnection;239;0;238;0
WireConnection;239;1;240;0
WireConnection;280;0;279;1
WireConnection;280;1;279;3
WireConnection;399;73;200;0
WireConnection;255;0;239;0
WireConnection;255;1;334;0
WireConnection;17;0;399;0
WireConnection;256;0;255;0
WireConnection;143;0;131;0
WireConnection;289;0;280;0
WireConnection;289;1;282;0
WireConnection;289;2;290;0
WireConnection;340;0;333;0
WireConnection;296;0;239;0
WireConnection;296;1;297;0
WireConnection;253;0;256;0
WireConnection;288;1;289;0
WireConnection;108;0;17;0
WireConnection;108;1;143;0
WireConnection;108;2;144;0
WireConnection;149;0;150;38
WireConnection;189;0;149;0
WireConnection;298;0;296;0
WireConnection;273;0;108;0
WireConnection;259;0;253;0
WireConnection;336;0;288;1
WireConnection;336;1;340;0
WireConnection;299;0;298;0
WireConnection;117;0;273;0
WireConnection;337;0;336;0
WireConnection;295;0;299;0
WireConnection;61;1;129;0
WireConnection;145;0;117;0
WireConnection;145;1;341;0
WireConnection;264;0;263;0
WireConnection;264;1;265;0
WireConnection;338;0;337;0
WireConnection;335;0;79;0
WireConnection;247;0;145;0
WireConnection;247;1;248;0
WireConnection;266;0;264;0
WireConnection;142;0;141;0
WireConnection;142;1;335;0
WireConnection;305;0;61;0
WireConnection;262;0;247;0
WireConnection;262;1;266;0
WireConnection;277;0;142;0
WireConnection;277;1;339;0
WireConnection;303;0;302;0
WireConnection;303;1;304;0
WireConnection;310;0;67;0
WireConnection;310;1;312;0
WireConnection;301;0;262;0
WireConnection;301;1;303;0
WireConnection;307;0;67;0
WireConnection;307;1;310;0
WireConnection;307;2;306;0
WireConnection;93;0;61;0
WireConnection;93;1;277;0
WireConnection;121;0;301;0
WireConnection;137;0;307;0
WireConnection;133;0;93;0
WireConnection;400;0;138;0
WireConnection;400;1;132;0
WireConnection;400;2;190;0
WireConnection;400;4;68;0
WireConnection;400;11;134;0
ASEEND*/
//CHKSM=945F1F8A8D94DEC3B5889FE87F15AB67AB75FABA