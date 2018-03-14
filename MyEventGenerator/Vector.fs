namespace MyEventGenerator

[<StructuredFormatDisplay("({X}, {Y}, {Z})")>]
type Vec3 = { X : double; Y : double; Z : double }
            with 
                 member v.Square = v.X * v.X + v.Y * v.Y + v.Z * v.Z
                 member v.Magnitude = sqrt v.Square
                 static member ToFourVector magnitude (v : Vec3) = { T = sqrt(v.Square + magnitude * magnitude); X = v.X; Y = v.Y; Z = v.Z }
                 static member Zero = { X = 0.; Y = 0.; Z = 0. }
                 static member (+) (v1, v2) = { X = v1.X + v2.X; Y = v1.Y + v2.Y; Z = v1.Z + v2.Z }
                 static member (-) (v1, v2) = { X = v1.X - v2.X; Y = v1.Y - v2.Y; Z = v1.Z - v2.Z }
                 static member (~-) v = { X = -v.X; Y = -v.Y; Z = -v.Z }
                 static member (*) (r, v) = { X = r * v.X; Y = r * v.Y; Z = r * v.Z }
                 static member (*) (v, r) = { X = v.X * r; Y = v.Y * r; Z = v.Z * r }
                 static member (/) (v, r) = { X = v.X / r; Y = v.Y / r; Z = v.Z / r }
                 static member ( *.) (v1, v2) = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z


and [<StructuredFormatDisplay("({T}; {X}, {Y}, {Z})")>] 
    Vec4 = { T : double; X : double; Y : double; Z : double }
        with
            member v.Vec3 = { X = v.X; Y = v.Y; Z = v.Z }
            static member Square v = v.T * v.T - v.Vec3.Square
            static member Boost (velocity : Vec3) (v : Vec4) = 
                let v2 = velocity.Square
                if v2 >= 1. then failwith "Trying to boost to faster than light"
                let g = 1. / sqrt(1. + v2)
                let v' = velocity + (g - 1.) * (v.Vec3 *. velocity) * velocity / v2 - g * v.T * velocity
                { T = g * (v.T - velocity *. v.Vec3); X = v'.X; Y = v'.Y; Z = v'.Z }
                                           
            static member Zero = { T = 0.; X = 0.; Y = 0.; Z = 0. }
            static member (+) (v1, v2) = { T = v1.T + v2.T; X = v1.X + v2.X; Y = v1.Y + v2.Y; Z = v1.Z + v2.Z }
            static member (-) (v1, v2) = { T = v1.T - v2.T; X = v1.X - v2.X; Y = v1.Y - v2.Y; Z = v1.Z - v2.Z }
            static member (~-) v = { T = -v.T; X = -v.X; Y = -v.Y; Z = -v.Z }
            static member (*) (r, v) = { T = r * v.T; X = r * v.X; Y = r * v.Y; Z = r * v.Z }
            static member (*) (v, r) = { T = v.T * r; X = v.X * r; Y = v.Y * r; Z = v.Z * r }
            static member (/) (v, r) = { T = v.T / r; X = v.X / r; Y = v.Y / r; Z = v.Z / r }