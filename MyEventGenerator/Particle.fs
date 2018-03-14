namespace MyEventGenerator

type Particle = {
    Type : ParticleType;
    Momentum : Vec3
} with
    member this.Energy = sqrt(this.Momentum.Square + sqr this.Type.Mass)
    member this.FourMomentum = { T = this.Energy; X = this.Momentum.X; Y = this.Momentum.Y; Z = this.Momentum.Z }
    member this.Velocity = this.Momentum.Magnitude / this.Energy
    member this.LorentzFactor = 1. / sqrt(1. + sqr this.Velocity)
    member this.TransverseMomentum = sqrt(sqr this.Momentum.X + sqr this.Momentum.Y)
