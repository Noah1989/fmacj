ASSEMBLY = ../bin/Fmacj.Framework.dll

FILES =  \
	AsynchronousAttribute.cs \
	ChannelAttribute.cs \
	ChordAttribute.cs \
	ForkAttribute.cs \
	IChannelEnumerable.cs \
	InconsistentChannelTypeException.cs \
	InvalidMethodException.cs \
	InvalidTypeException.cs \
	IParallelizable.cs \
	JoinAttribute.cs \
	MovableAttribute.cs \
	ParallelizableAttribute.cs \
	ParallelizationException.cs \
	Properties/AssemblyInfo.cs \
	RemoteException.cs \
	TypeNotFoundException.cs \
	TypeValidator.cs \
	YieldAttribute.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
