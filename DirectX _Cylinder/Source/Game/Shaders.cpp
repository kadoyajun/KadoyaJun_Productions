#include "Game.h"
#include "BasicVertexShader.h"
#include "BasicPixelShader.h"
#include "BasicGeometryShader.h"
#include <comdef.h>

BasicVertexShader::BasicVertexShader(ID3D11Device* graphicsDevice) {

	const auto hr = graphicsDevice->CreateVertexShader(
		g_BasicVertexShader,
		ARRAYSIZE(g_BasicVertexShader),
		NULL,
		&shader);
	if (FAILED(hr)) {
		OutputDebugString(L"頂点シェーダーの作成に失敗しました。");
		throw _com_error(hr);
	}
}

ID3D11VertexShader* BasicVertexShader::GetNativePointer()
{
	return shader.Get();
}


const BYTE* BasicVertexShader::GetBytecode()
{
	return g_BasicVertexShader;
}

SIZE_T BasicVertexShader::GetBytecodeLength()
{
	return ARRAYSIZE(g_BasicVertexShader);
}


BasicGeometryShader::BasicGeometryShader(ID3D11Device* graphicsDevice)
{
	const auto hr = graphicsDevice->CreateGeometryShader(
		g_BasicGeometryShader, ARRAYSIZE(g_BasicGeometryShader),
		NULL,
		&shader);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
}


ID3D11GeometryShader* BasicGeometryShader::GetNativePointer()
{
	return shader.Get();
}

BasicPixelShader::BasicPixelShader(ID3D11Device* graphicsDevice)
{
	const auto hr = graphicsDevice->CreatePixelShader(
		g_BasicPixelShader, ARRAYSIZE(g_BasicPixelShader),
		NULL,
		&shader);
	if (FAILED(hr)) {
		throw _com_error(hr);
	}
}

ID3D11PixelShader* BasicPixelShader::GetNativePointer()
{
	return shader.Get();
}
