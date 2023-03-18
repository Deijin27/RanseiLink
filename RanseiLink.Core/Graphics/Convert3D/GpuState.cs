#nullable enable
using System;
using System.Numerics;

namespace RanseiLink.Core.Graphics;

public class GpuState
{
    public Matrix4x4[] MatrixStack { get; set; }
    public Matrix4x4 CurrentMatrix { get; set; }
    public NSMDL.Model.Material CurrentMaterial { get; set; }

    public GpuState(NSMDL.Model.Material initMaterial)
    {
        CurrentMaterial = initMaterial;
        CurrentMatrix = Matrix4x4.Identity;
        MatrixStack = new Matrix4x4[32];
        for (int i = 0; i < MatrixStack.Length; i++)
        {
            MatrixStack[i] = Matrix4x4.Identity;
        }
    }

    public void Restore(int stackIndex)
    {
        CurrentMatrix = MatrixStack[stackIndex];
    }

    public void Store(int stackIndex)
    {
        MatrixStack[stackIndex] = CurrentMatrix;
    }

    public void MultiplyMatrix(Matrix4x4 mtx)
    {
        CurrentMatrix = mtx * CurrentMatrix;
    }
}


public class InverseGpuState
{
    public Matrix4x4[] MatrixStack { get; set; }
    public Matrix4x4 CurrentMatrix { get; set; }
    public Matrix4x4 InverseCurrentMatrix { get; set; }
    public NSMDL.Model.Material CurrentMaterial { get; set; }

    public InverseGpuState(NSMDL.Model.Material initMaterial)
    {
        CurrentMaterial = initMaterial;
        CurrentMatrix = Matrix4x4.Identity;
        InverseCurrentMatrix = Matrix4x4.Identity;
        MatrixStack = new Matrix4x4[32];
        for (int i = 0; i < MatrixStack.Length; i++)
        {
            MatrixStack[i] = Matrix4x4.Identity;
        }
    }

    public void Restore(int stackIndex)
    {
        CurrentMatrix = MatrixStack[stackIndex];
        if (!Matrix4x4.Invert(CurrentMatrix, out var inv))
        {
            throw new Exception("Failed to invert matrix");
        }
        InverseCurrentMatrix = inv;
    }

    public void Store(int stackIndex)
    {
        MatrixStack[stackIndex] = CurrentMatrix;
    }

    public void MultiplyMatrix(Matrix4x4 mtx)
    {
        CurrentMatrix = mtx * CurrentMatrix;
        if (!Matrix4x4.Invert(CurrentMatrix, out var inv))
        {
            throw new Exception("Failed to invert matrix");
        }
        InverseCurrentMatrix = inv;
    }
}