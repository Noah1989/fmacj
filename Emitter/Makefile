ASSEMBLY = ../bin/Fmacj.Emitter.dll

FILES =  \
	Properties/AssemblyInfo.cs \
	AssemblyParallelizer.cs \
	ChordImplementer.cs \
	ChordInfo.cs \
	ForkGroup.cs \
	ForkImplementer.cs \
	ParallelizationFactory.cs \
	TypeAnalyzer.cs \
	TypeParallelizer.cs

REFERENCES = \
	-r:../bin/Fmacj.Framework.dll \
	-r:../bin/Fmacj.Runtime.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES) 

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
