
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Fmacj.Components.TaskClient.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Core.Framework/bin/Debug/Fmacj.Core.Framework.dll
BUILD_DIR = bin/Debug

FMACJ_CORE_FRAMEWORK_DLL_SOURCE=../Core.Framework/bin/Debug/Fmacj.Core.Framework.dll
FMACJ_CORE_FRAMEWORK_DLL=$(BUILD_DIR)/Fmacj.Core.Framework.dll
FMACJ_CORE_FRAMEWORK_DLL_MDB_SOURCE=../Core.Framework/bin/Debug/Fmacj.Core.Framework.dll.mdb
FMACJ_CORE_FRAMEWORK_DLL_MDB=$(BUILD_DIR)/Fmacj.Core.Framework.dll.mdb
FMACJ_COMPONENTS_TASKCLIENT_DLL_MDB_SOURCE=bin/Debug/Fmacj.Components.TaskClient.dll.mdb
FMACJ_COMPONENTS_TASKCLIENT_DLL_MDB=$(BUILD_DIR)/Fmacj.Components.TaskClient.dll.mdb
FRAMEWORK_DLL=

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Components.TaskClient.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Core.Framework/bin/Release/Framework.dll
BUILD_DIR = bin/Release

FMACJ_CORE_FRAMEWORK_DLL=
FMACJ_CORE_FRAMEWORK_DLL_MDB=
FMACJ_COMPONENTS_TASKCLIENT_DLL_MDB=
FRAMEWORK_DLL_SOURCE=../Core.Framework/bin/Release/Framework.dll
FRAMEWORK_DLL=$(BUILD_DIR)/Framework.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FMACJ_CORE_FRAMEWORK_DLL) \
	$(FMACJ_CORE_FRAMEWORK_DLL_MDB) \
	$(FMACJ_COMPONENTS_TASKCLIENT_DLL_MDB) \
	$(FRAMEWORK_DLL)  


RESGEN=resgen2


FILES = \
	TaskClient.cs \
	Properties/AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System.Core \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,FMACJ_CORE_FRAMEWORK_DLL))
$(eval $(call emit-deploy-target,FMACJ_CORE_FRAMEWORK_DLL_MDB))
$(eval $(call emit-deploy-target,FRAMEWORK_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FMACJ_CORE_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FMACJ_CORE_FRAMEWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FMACJ_COMPONENTS_TASKCLIENT_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FMACJ_CORE_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FMACJ_CORE_FRAMEWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FMACJ_COMPONENTS_TASKCLIENT_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
