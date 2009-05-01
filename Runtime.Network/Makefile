ASSEMBLY = ../bin/Fmacj.Runtime.Network.dll

FILES =  \
	Communication/DistributionServerRequest.cs \
	Communication/DistributionServerResponse.cs \
	Communication/ExceptionResponse.cs \
	Communication/RunTaskRequest.cs \
	Properties/AssemblyInfo.cs

REFERENCES =

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
