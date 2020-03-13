# Brainfuck.NET

A Brainfuck compiler targeting .NET core.

Brainfuck is an esoteric language developed by Urban Müller in 1993. Read more
about it on [Wikipedia](https://wikipedia.org/wiki/Brainfuck) or on
[esolangs](https://esolangs.org/wiki/Brainfuck).

## Why?

Have you ever felt limited when programming a .NET application? Is C# just too
verbose? F# just too complicated? VB too just old? Well, now you can use
everyone's favorite language: Brainfuck!

Compile either to stand-alone executable or, more usefully, to a library file
you can link to from other .NET projects.  
With the ability to compile Brainfuck to .NET libraries you can write that
complicated/performance demanding/crucial function in Brainfuck! Without having
to rewrite your whole project.

## How?

To compile your own Brainfuck programs, head to the [release page](./releases)
and download the latest version.

### Usage Example

Compile a Brainfuck source file (`hello.bf`) to an executable.
```
> BrainfuckNET hello.bf exe
```

Compile `cat.bf` to a library where the code is accessible from
`BFNamespace.BFClass.BFMethod`.
```
> BrainfuckNET cat.bf dll
```

Compile `cat.bf` to a library but the code is now accessible from
`Repeater.Repeating.Cat` instead.
```
> BrainfuckNET cat.bf dll Repeater.Repeating.Cat
```