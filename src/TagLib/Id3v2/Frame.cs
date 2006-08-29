/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private FrameHeader header;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public void SetData (ByteVector data)
      {
         Parse (data);
      }
      
      public virtual void SetText (string text) {}
      
      public ByteVector Render ()
      {
         ByteVector field_data = RenderFields ();
         header.FrameSize = (uint) field_data.Count;
         ByteVector header_data = header.Render ();

         return header_data + field_data;
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public ByteVector FrameId {get {return (header != null) ? header.FrameId : null;}}
      public uint       Size    {get {return (header != null) ? header.FrameSize : 0;}}
      
      //////////////////////////////////////////////////////////////////////////
      // public static methods
      //////////////////////////////////////////////////////////////////////////
      public static ByteVector TextDelimiter (StringType t)
      {
         return new ByteVector ((t == StringType.UTF16 || t == StringType.UTF16BE) ? 2 : 1, (byte) 0);
      }

      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected Frame (ByteVector data)
      {
         header = new FrameHeader (data);
      }
      
      protected Frame (FrameHeader header)
      {
         this.header = header;
      }
      
      protected internal FrameHeader Header
      {
         get {return header;}
         set {header = value;}
      }
      
      protected void Parse (ByteVector data)
      {
         if (header != null)
            header.SetData (data);
         else
            header = new FrameHeader (data);
         
         ParseFields (FieldData (data));
      }
      
      protected virtual void ParseFields(ByteVector data) {}
      protected virtual ByteVector RenderFields () {return new ByteVector ();}
      protected ByteVector FieldData (ByteVector frame_data)
      {
         uint header_size = FrameHeader.Size (header.Version);

         uint frame_data_offset = header_size;
         uint frame_data_length = Size;

         if (header.Compression || header.DataLengthIndicator)
         {
            frame_data_length = frame_data.Mid ((int) header_size, 4).ToUInt ();
            frame_data_length += 4;
         }
         
         // FIXME: Impliment compression and encrpytion.
         /*
         #if HAVE_ZLIB
            if(d->header->compression()) {
               ByteVector data(frameDataLength);
               uLongf uLongTmp = frameDataLength;
               ::uncompress((Bytef *) data.data(),
                           (uLongf *) &uLongTmp,
                           (Bytef *) frameData.data() + frameDataOffset,
                           size());
               return data;
            }
            else
         #endif
         */
         
         return frame_data.Mid ((int) frame_data_offset, (int) frame_data_length);
      }
   }
}