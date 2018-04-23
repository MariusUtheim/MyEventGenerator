namespace MyEventGenerator
open System.Collections.Generic
open System.Linq

type EventEntry(event: Event, index : int, particle : Particle, mother : int) =
    
    let _children : List<_> = List<EventEntry>()

    member this.AddChild entry = _children.Add(entry)
    member this.Branch(particles) = _children.AddRange([ for particle in particles -> event.WriteEntry (particle, this) ])
    member this.Particle = particle
    member this.Event = event
    member this.Index = index
    member this.Mother = mother
    member this.IsInitial = (mother = 0)
    member this.IsFinal = not(_children.Any())




and Event() =

    let entries = LinkedList<EventEntry>()
    
    new (particles) as this = Event() then for particle in particles do this.WriteEntry(particle) |> ignore
                      

    member this.WriteEntry (particle, mother) = assert (mother.Event = this)
                                                let newEntry = EventEntry(this, entries.Count, particle, mother.Index)
                                                mother.AddChild(newEntry)
                                                entries.AddLast(newEntry) |> ignore
                                                newEntry

    member this.WriteEntry particle = let newEntry = EventEntry(this, entries.Count, particle, -1)
                                      entries.AddLast(newEntry) |> ignore
                                      newEntry

    member this.Entries = entries.AsEnumerable()

    member this.ForAllIteratively f =
        let mutable node = entries.First
        while node <> null do 
            f node.Value
            node <- node.Next

    member this.ForAllOnce f =
        let entries = this.Entries.ToList()
        for e in entries do f e

    static member Print (event : Event) =
        printfn "Idx\tMother\tPdg\tEnergy"
        for entry in event.Entries do printfn "%s%A\t%d\t%A\t%A" (if entry.IsFinal then "*" else "") entry.Index entry.Mother entry.Particle.Type.PdgId entry.Particle.FourMomentum

        let totalMomentum = event.Entries 
                            |> Seq.where (fun entry -> entry.IsFinal)
                            |> Seq.sumBy (fun (entry : EventEntry) -> entry.Particle.FourMomentum)
        printfn "Total momentum: %A" totalMomentum
            
