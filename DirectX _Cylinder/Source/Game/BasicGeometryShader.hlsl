#include "BasicShader.hlsli"

[maxvertexcount(3)]
void main(
	triangle GSInput input[3],
	inout TriangleStream<GSOutput> output
)
{
	[unroll]
    for (uint i = 0; i < 3; i++)
    {
        GSOutput element;
        element.position = mul(input[i].position, WorldViewProjection);
        element.worldPosition = mul(input[i].position, World);
        element.worldNormal = mul(input[i].normal, (float3x3) World);
        output.Append(element);
    }
}
