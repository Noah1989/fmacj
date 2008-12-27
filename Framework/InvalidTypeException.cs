/*
    FMACJ Parallelization Framework for .NET
    Copyright (C) 2008  Stefan Noack

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Fmacj.Framework
{
    public class InvalidTypeException : ParallelizationException
    {
        public InvalidTypeException()
            : base(
                "A type is marked as [Parallelizable] but does not implement IParallelizable or is not an abstract class."
                )
        {
        }

        public InvalidTypeException(string typeName)
            : base(
                string.Format(
                    "The type '{0}' is marked as [Parallelizable] but does not implement IParallelizable or is not an abstract class.",
                    typeName))
        {
        }
    }
}