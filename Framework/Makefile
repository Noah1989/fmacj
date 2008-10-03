ASSEMBLY = ../bin/Fmacj.Framework.dll

FILES =  \
	Properties/AssemblyInfo.cs \
	AsynchronousAttribute.cs \
	ChannelAttribute.cs \
	ChordAttribute.cs \
	ForkAttribute.cs \
	IParallelizable.cs \
	InvalidMethodException.cs \
	InvalidTypeException.cs \
	JoinAttribute.cs \
	MovableAttribute.cs \
	ParallelizableAttribute.cs \
	ParallelizationException.cs \
	TypeNotFoundException.cs \
	TypeValidator.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
