cbuffer ConstantBuffer
{
    matrix World; //ワールド変換行列
    matrix View; //ビュー変換行列
    matrix Projection; //透視射影変換行列
    matrix WorldViewProjection; // WVP行列
    
	// カメラの位置座標
    float4 ViewPosition;
	// ライトの位置座標(平行光源 w = 0, 点光源 w = 1)
    float4 LightPosition;
	// マテリアルの表面カラー
    float4 MaterialDiffuse;

	// マテリアルの鏡面反射カラー
    float3 MaterialSpecularColor;
	// マテリアルの鏡面反射の強さ
    float MaterialSpecularPower;
}

// 頂点シェーダーの入力データ
struct VSInput
{
    float4 position : POSITION;
    float3 normal : NORMAL;
};

// 頂点シェーダーの出力データ
struct VSOutput
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
};

// ジオメトリーシェーダーの入力データ
typedef VSOutput GSInput;

// ジオメトリーシェーダーの出力データ
struct GSOutput
{
    float4 position : SV_POSITION;
    float4 worldPosition : POSITION;
    float3 worldNormal : NORMAL;
};

// ピクセルシェーダーの入力データ
typedef GSOutput PSInput;
