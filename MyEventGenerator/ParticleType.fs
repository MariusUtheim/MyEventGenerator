namespace MyEventGenerator


type ParticleType = { 
     PdgId : int; 
     Mass : float;
     Charge : float;
     } with
         member this.AbsId = abs this.PdgId
         member this.IsQuark = this.AbsId <= 9
         member this.IsParton = this.IsQuark || this.AbsId = 21
         static member (~~) p = { p with PdgId = -p.PdgId; Charge = -p.Charge; }

type DecayMode = ParticleType list * float


module Particles =
    
    let Down = {
        PdgId = 1;
        Mass = 0.0047;
        Charge = -1./3.;
    }

    let Up = {
        PdgId = 2;
        Mass = 0.0022;
        Charge = 2./3.;
    }

    let ElectronNeutrino = {
        PdgId = 12;
        Mass = 0.0;
        Charge = 0.;
    }

    let MuonNeutrino = {
        PdgId = 14;
        Mass = 0.0;
        Charge = 0.;
    }

    let TauNeutrino = {
        PdgId = 16;
        Mass = 0.0;
        Charge = 0.;
    }

    let Electron = { 
        PdgId = 11;
        Mass = 0.000511; 
        Charge = -1.;
    }

    let Muon = { 
        PdgId = 13; 
        Mass = 0.10667; 
        Charge = -1.;
    }

    let Tau = { 
        PdgId = 15; 
        Mass = 1.17768; 
        Charge = -1.;
    }
    
    let Gluon = {
        PdgId = 21;
        Mass = 0.;
        Charge = 0.;
    }

    let Photon = {
        PdgId = 22;
        Mass = 0.;
        Charge = 0.;
    }