ASSEMBLY = ../bin/Fmacj.Emitter.dll

FILES =  \
	AssemblyParallelizer.cs \
	ChannelImplementer.cs \
	ChordImplementer.cs \
	ChordInfo.cs \
	ForkGroup.cs \
	ForkImplementer.cs \
	JoinGroup.cs \
	JoinImplementer.cs \
	ParallelizationFactory.cs \
	Properties/AssemblyInfo.cs \
	TypeAnalyzer/TypeAnalyzer.Chord.cs \
	TypeAnalyzer/TypeAnalyzer.cs \
	TypeAnalyzer/TypeAnalyzer.Fork.cs \
	TypeAnalyzer/TypeAnalyzer.Join.cs \
	TypeAnalyzer/TypeAnalyzer.Yield.cs \
	TypeParallelizer.cs \
	YieldImplementer.cs \
	YieldInfo.cs

REFERENCES =  \
	-r:../bin/Fmacj.Framework.dll \
	-r:../bin/Fmacj.Runtime.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -d:DEBUG -debug $(FILES) 

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
