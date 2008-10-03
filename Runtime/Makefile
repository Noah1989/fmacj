ASSEMBLY = ../bin/Fmacj.Runtime.dll

FILES =  \
	Properties/AssemblyInfo.cs \
	Bus.cs \
	Channel.cs \
	ChannelFactory.cs \
	ChordManager.cs \
	IChannel.cs \

REFERENCES = -r:../bin/Fmacj.Framework.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
