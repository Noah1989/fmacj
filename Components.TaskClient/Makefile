ASSEMBLY = ../bin/Fmacj.Components.TaskClient.dll

FILES =  \
	Properties/AssemblyInfo.cs \
	TaskClient.cs

REFERENCES =  \
	-r:../bin/Fmacj.Framework.dll \
	-r:../bin/Fmacj.Runtime.Network.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
