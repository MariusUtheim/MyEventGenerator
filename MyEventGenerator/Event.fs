namespace MyEventGenerator
open System.Collections.Generic
open System.Linq

type EventEntry(event: Event, index : int, particle : Particle, mother : int) =
    
    let _children : List<_> = List<EventEntry>()

    member this.Branch(particles) = _children.AddRange([ for particle in particles -> event.WriteEntry (particle, this) ])
    member this.Particle = particle
    member this.Event = event
    member this.Index = index
    member this.Mother = mother

and Event() =

    let entries = LinkedList<EventEntry>()
    
    new (particles) as this = Event() then for particle in particles do this.WriteEntry(particle) |> ignore
                      

    member this.WriteEntry (particle, mother) = assert (mother.Event = this)
                                                let newEntry = EventEntry(this, entries.Count, particle, mother.Index)
                                                entries.AddLast(newEntry) |> ignore
                                                newEntry

    member this.WriteEntry particle = let newEntry = EventEntry(this, entries.Count, particle, 0)
                                      entries.AddLast(newEntry) |> ignore
                                      newEntry

    member this.Entries = entries.AsEnumerable()

    member this.ForAllIteratively f =
        let mutable node = entries.First
        while node <> null do 
            f node.Value
            node <- node.Next

    static member Print (event : Event) =
        printfn "Idx\tPdg\tEnergy"
        for entry in event.Entries do printfn "%A\t%A\t%A" entry.Index entry.Particle.Type.PdgId entry.Particle.FourMomentum

        printfn "Total momentum: %A" <| Seq.sumBy (fun (entry : EventEntry) -> entry.Particle.FourMomentum) event.Entries
