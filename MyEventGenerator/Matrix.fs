namespace MyEventGenerator


type Matrix(m00, m01, m02, m10, m11, m12, m20, m21, m22) =

    member this.Apply ({ X = x; Y = y; Z = z } : Vec3) =
        { X = x * m00 + y * m01 + z * m02; 
          Y = x * m10 + y * m11 + z * m12;
          Z = x * m20 + y * m21 + z * m22; }

    static member Rotation axis angle =
        let ({ X = ux; Y = uy; Z = uz } : Vec3) = Vec3.Normalized axis
        let c, c' = cos angle, (1. - cos angle)
        let s, s' = sin angle, (1. - sin angle)
        Matrix(c + ux * ux * c',      ux * uy * c' - uz * s, ux * uz * c' + uy * s,
               uy * ux * c' + uz * s, c + uy * uy * c',      uy * uz * c' - ux * s,
               uz * ux * c' - uy * s, uz * uy * c' + ux * s, c + uz * uz * c')