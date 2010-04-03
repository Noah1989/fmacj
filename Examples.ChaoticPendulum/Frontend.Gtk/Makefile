
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/ChaoticPendulum.Frontend.Gtk.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../Core/bin/Debug/ChaoticPendulum.Core.dll \
	../Visualization/bin/Debug/ChaoticPendulum.Visualization.dll
BUILD_DIR = bin/Debug

CHAOTICPENDULUM_CORE_DLL_SOURCE=../Core/bin/Debug/ChaoticPendulum.Core.dll
CHAOTICPENDULUM_CORE_DLL_MDB_SOURCE=../Core/bin/Debug/ChaoticPendulum.Core.dll.mdb
CHAOTICPENDULUM_CORE_DLL_MDB=$(BUILD_DIR)/ChaoticPendulum.Core.dll.mdb
CHAOTICPENDULUM_VISUALIZATION_DLL_SOURCE=../Visualization/bin/Debug/ChaoticPendulum.Visualization.dll
CHAOTICPENDULUM_FRONTEND_GTK_EXE_MDB_SOURCE=bin/Debug/ChaoticPendulum.Frontend.Gtk.exe.mdb
CHAOTICPENDULUM_FRONTEND_GTK_EXE_MDB=$(BUILD_DIR)/ChaoticPendulum.Frontend.Gtk.exe.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/ChaoticPendulum.Frontend.Gtk.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../Core/bin/Release/ChaoticPendulum.Core.dll \
	../Visualization/bin/Release/ChaoticPendulum.Visualization.dll
BUILD_DIR = bin/Release

CHAOTICPENDULUM_CORE_DLL_SOURCE=../Core/bin/Release/ChaoticPendulum.Core.dll
CHAOTICPENDULUM_CORE_DLL_MDB=
CHAOTICPENDULUM_VISUALIZATION_DLL_SOURCE=../Visualization/bin/Release/ChaoticPendulum.Visualization.dll
CHAOTICPENDULUM_FRONTEND_GTK_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(CHAOTICPENDULUM_CORE_DLL) \
	$(CHAOTICPENDULUM_CORE_DLL_MDB) \
	$(CHAOTICPENDULUM_VISUALIZATION_DLL) \
	$(CHAOTICPENDULUM_FRONTEND_GTK_EXE_MDB)  


RESGEN=resgen2

CHAOTICPENDULUM_CORE_DLL = $(BUILD_DIR)/ChaoticPendulum.Core.dll
CHAOTICPENDULUM_VISUALIZATION_DLL = $(BUILD_DIR)/ChaoticPendulum.Visualization.dll

FILES =  \
	AssemblyInfo.cs \
	gtk-gui/generated.cs \
	gtk-gui/MainWindow.cs \
	Main.cs \
	MainWindow.cs 

DATA_FILES = 

RESOURCES = gtk-gui/gui.stetic 

EXTRAS = 

REFERENCES =  \
	../Core/bin/Debug/ChaoticPendulum.Core.dll \
	../Visualization/bin/Debug/ChaoticPendulum.Visualization.dll \
	Mono.Posix \
	-pkg:glade-sharp-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-dotnet-2.0 \
	-pkg:gtk-sharp-2.0 \
	System \
	System.Drawing

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,CHAOTICPENDULUM_CORE_DLL))
$(eval $(call emit-deploy-target,CHAOTICPENDULUM_CORE_DLL_MDB))
$(eval $(call emit-deploy-target,CHAOTICPENDULUM_VISUALIZATION_DLL))


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
	$(call cp,$(CHAOTICPENDULUM_CORE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(CHAOTICPENDULUM_CORE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(CHAOTICPENDULUM_VISUALIZATION_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(CHAOTICPENDULUM_FRONTEND_GTK_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CHAOTICPENDULUM_CORE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CHAOTICPENDULUM_CORE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CHAOTICPENDULUM_VISUALIZATION_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(CHAOTICPENDULUM_FRONTEND_GTK_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
