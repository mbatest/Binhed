using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace BluRay
{
    class BDJO
    {
        object result;
        void _get_string(BitStreamReader buf, string outs, int length)
        {
            outs = buf.ReadString(length);
        }
        string _read_string(JNIEnv env, BitStreamReader buf, int length)
        {
            string str = buf.ReadString(length);
            string jstr = (env).NewStringUTF(env, str);
            return jstr;
        }
        object _parse_terminal_info(JNIEnv env, BitStreamReader buf)
        {
            // SkipBit length specifier
            buf.SkipBit(32);

            string default_font = buf.ReadString(5);
            string jdefault_font = (env).NewStringUTF(env, default_font);

            int havi_config = buf.ReadIntFromBits(4);
            bool menu_call_mask = buf.ReadBool();
            bool title_search_mask = buf.ReadBool();

            buf.BitPosition = 34 * 8;//.seek(buf, 34);

            return bdj_make_object(env, "org/videolan/bdjo/TerminalInfo", new object[]{"(Ljava/lang/String;IZZ)V",
                    jdefault_font, havi_config, menu_call_mask, title_search_mask});
        }
        object _parse_app_cache_info(JNIEnv env, BitStreamReader buf)
        {
            // SkipBit length specifier
            buf.SkipBit(32);//buf.SkipBit(32);

            int count = buf.ReadByte();

            object app_cache_array = bdj_make_array(env, "org/videolan/bdjo/AppCache", count);
            JNICHK(app_cache_array);

            // SkipBit padding
            buf.SkipBit(8);


            for (int i = 0; i < count; i++)
            {
                int type = buf.ReadByte();

                string ref_to_name = buf.ReadString(5);
                string jref_to_name = (env).NewStringUTF(env, ref_to_name);
                JNICHK(jref_to_name);

                string language_code = buf.ReadString(3);
                string jlanguage_code = (env).NewStringUTF(env, language_code);
                JNICHK(jlanguage_code);

                object entry = bdj_make_object(env, "org/videolan/bdjo/AppCache",new object[]{
                        "(ILjava/lang/String;Ljava/lang/String;)V", type, jref_to_name, jlanguage_code});
                JNICHK(entry);

                (env).SetObjectArrayElement(env, app_cache_array, i, entry);

                // SkipBit padding
                buf.SkipBit(24);
            }
            return app_cache_array;
        }
        object _parse_accessible_playlists(JNIEnv env, BitStreamReader buf)
        {
            // SkipBit length specifier
            buf.SkipBit(32);

            int count = buf.ReadIntFromBits(11);
            object playlists = bdj_make_array(env, "java/lang/String", count);

            bool access_to_all_flag = buf.ReadBool();
            bool autostart_first_playlist = buf.ReadBool();

            // SkipBit padding
            buf.SkipBit(19);

            for (int i = 0; i < count; i++)
            {
                string playlist_name = buf.ReadString(5);
                string jplaylist_name = (env).NewStringUTF(env, playlist_name);
                JNICHK(jplaylist_name);

                (env).SetObjectArrayElement(env, playlists, i, jplaylist_name);

                // SkipBit padding
                buf.SkipBit(8);
            }

            return bdj_make_object(env, "org/videolan/bdjo/PlayListTable",new object[]{ "(ZZ[Ljava/lang/String;)V",
                    access_to_all_flag, autostart_first_playlist, playlists});
        }
        object _parse_app_management_table(JNIEnv env, BitStreamReader buf)
        {
            // SkipBit length specifier
            buf.SkipBit(32);

            int count = buf.ReadByte();

            object entries = bdj_make_array(env, "org/videolan/bdjo/AppEntry", count);

            // SkipBit padding
            buf.SkipBit(8);

            for (int i = 0; i < count; i++)
            {
                int control_code = buf.ReadByte();
                int type = buf.ReadIntFromBits(4);

                // SkipBit padding
                buf.SkipBit(4);

                int organization_id = buf.ReadInteger();
                short application_id = buf.ReadShort();

                // SkipBit padding
                buf.SkipBit(80);

                int profiles_count = buf.ReadIntFromBits(4);
                object profiles = bdj_make_array(env, "org/videolan/bdjo/AppProfile", profiles_count);
                JNICHK(profiles);

                // SkipBit padding
                buf.SkipBit(12);

                for (int j = 0; j < profiles_count; j++)
                {
                    short profile_num = buf.ReadShort();
                    byte major_version = buf.ReadByte();
                    byte minor_version = buf.ReadByte();
                    byte micro_version = buf.ReadByte();

                    object profile = bdj_make_object(env, "org/videolan/bdjo/AppProfile", new object[]{"(SBBB)V",
                            profile_num, major_version, minor_version, micro_version});
                    JNICHK(profile);

                    (env).SetObjectArrayElement(env, profiles, j, profile);
                    JNICHK(1);

                    // SkipBit padding
                    buf.SkipBit(8);
                }

                int priority = buf.ReadByte();
                int binding = buf.ReadIntFromBits(2);
                int visibility = buf.ReadIntFromBits(2);

                // SkipBit padding
                buf.SkipBit(4);

                short name_data_length = buf.ReadShort();
                object app_names = null;

                if (name_data_length > 0)
                {
                    // first scan for the number of app names
                    int app_name_count = 0;
                    int name_bytes_read = 0;
                    while (name_bytes_read < name_data_length)
                    {
                        buf.SkipBit(24);

                        byte name_length = buf.ReadByte();
                        buf.SkipBit(name_length);

                        app_name_count++;
                        name_bytes_read += 4 + name_length;
                    }

                    // seek back to beginning of names
                    buf.SkipBit(-name_data_length);

                    app_names = bdj_make_array(env, "org/videolan/bdjo/AppName", app_name_count);
                    JNICHK(app_names);

                    for (int j = 0; j < app_name_count; j++)
                    {
                        string language = buf.ReadString(3);
                        string jlanguage = (env).NewStringUTF(env, language);
                        JNICHK(jlanguage);

                        byte name_length = buf.ReadByte();
                        string jname = _read_string(env, buf, name_length);
                        JNICHK(jname);

                        object app_name = bdj_make_object(env, "org/videolan/bdjo/AppName",new object[]{"(Ljava/lang/String;Ljava/lang/String;)V", jlanguage, jname});
                        JNICHK(app_name);

                        (env).SetObjectArrayElement(env, app_names, i, app_name);
                        JNICHK(1);
                    }
                }

                // SkipBit padding to word boundary
                if ((name_data_length & 0x1) != 0)
                {
                    buf.SkipBit(8);
                }

                byte icon_locator_length = buf.ReadByte();
                string icon_locator = _read_string(env, buf, icon_locator_length);
                JNICHK(icon_locator);

                // SkipBit padding to word boundary
                if ((icon_locator_length & 0x1) == 0)
                {
                    buf.SkipBit(8);
                }

                short icon_flags = buf.ReadShort();

                byte base_dir_length = buf.ReadByte();
                string base_dir = _read_string(env, buf, base_dir_length);
                JNICHK(base_dir);

                // SkipBit padding to word boundary
                if ((base_dir_length & 0x1) == 0)
                {
                    buf.SkipBit(8);
                }

                byte classpath_length = buf.ReadByte();
                string classpath_extension = _read_string(env, buf, classpath_length);
                JNICHK(classpath_extension);

                // SkipBit padding to word boundary
                if ((classpath_length & 0x1) == 0)
                {
                    buf.SkipBit(8);
                }

                byte initial_class_length = buf.ReadByte();
                string initial_class = _read_string(env, buf, initial_class_length);

                // SkipBit padding to word boundary
                if ((initial_class_length & 0x1) == 0)
                {
                    buf.SkipBit(8);
                }

                byte param_data_length = buf.ReadByte();

                object Params = null;
                if (param_data_length > 0)
                {
                    // first scan for the number of Params
                    int param_count = 0;
                    int param_bytes_read = 0;
                    while (param_bytes_read < param_data_length)
                    {
                        byte param_length = buf.ReadByte();
                        buf.SkipBit(8);

                        param_count++;
                        param_bytes_read += 1 + param_length;
                    }

                    // seek back to beginning of Params
                    buf.SkipBit(-param_data_length);

                    Params = bdj_make_array(env, "java/lang/String", param_count);

                    for (int j = 0; j < param_count; j++)
                    {
                        byte param_length = buf.ReadByte();
                        string param = _read_string(env, buf, param_length);
                        JNICHK(param);

                        (env).SetObjectArrayElement(env, Params, i, param);
                        JNICHK(1);
                    }
                }

                // SkipBit padding to word boundary
                if ((param_data_length & 0x1) == 0)
                {
                    buf.SkipBit(8);
                }

                object entry = bdj_make_object(env, "org/videolan/bdjo/AppEntry",new object[]{
                        "(IIIS[Lorg/videolan/bdjo/AppProfile;SII[Lorg/videolan/bdjo/AppName;Ljava/lang/String;SLjava/lang/String;Ljava/lang/String;Ljava/lang/String;[Ljava/lang/String;)V",
                        control_code, type, organization_id, application_id, profiles,
                        priority, binding, visibility, app_names, icon_locator,
                        icon_flags, base_dir, classpath_extension, initial_class, Params});
                JNICHK(entry);

                (env).SetObjectArrayElement(env, entries, i, entry);
            }

            return entries;
        }
        object _parse_bdjo(JNIEnv env, BitStreamReader buf)
        {
            // first check magic number
            string magic = buf.ReadString(4); ;
            if (magic != "BDJO")
            {
                return null;
            }

            // get version string
            string _version = buf.ReadString(4);// 0100 ou 0200
            if ((_version != "0100") && (_version != "0200"))
            {
                return null;
            }

            // SkipBit some unnecessary data
            buf.SkipBit(8 * 0x28);

            object terminal_info = _parse_terminal_info(env, buf);
            JNICHK(terminal_info);
            object app_cache_info = _parse_app_cache_info(env, buf);
            JNICHK(app_cache_info);
            object accessible_playlists = _parse_accessible_playlists(env, buf);
            JNICHK(accessible_playlists);
            object app_table = _parse_app_management_table(env, buf);
            JNICHK(app_table);
            int key_interest_table = buf.ReadInteger();
            short file_access_length = buf.ReadShort();
            string file_access_info = _read_string(env, buf, file_access_length);
            JNICHK(file_access_info);

            return bdj_make_object(env, "org/videolan/bdjo/Bdjo",new object[]{
                    "(Lorg/videolan/bdjo/TerminalInfo;[Lorg/videolan/bdjo/AppCache;Lorg/videolan/bdjo/PlayListTable;[Lorg/videolan/bdjo/AppEntry;ILjava/lang/String;)V",
                    terminal_info, app_cache_info, accessible_playlists, app_table, key_interest_table, file_access_info});
        }
        public BDJO(JNIEnv env, string fileName)
        {
            //http://www.hdcookbook.com/
            MessageBox.Show("To be debugged");
            BitStreamReader buf = new BitStreamReader(fileName, true);
            if (buf.Length <= 0)
            {
                return;
            }
            else
            {
                result = _parse_bdjo(env, buf);
            }
        }
        object bdj_make_array(JNIEnv env, string name, int count)
        {
            return null;
        }
        object bdj_make_object(JNIEnv env, string name, object sig)
        {
            return null;
        }
        void JNICHK(object o) { }
    }
    public class JNIEnv
    {
        public string NewStringUTF(JNIEnv a, string b)
        {
            return b;
        }
        public void SetObjectArrayElement(JNIEnv a,object b, object c, object d)
        {
        }
    }
    class jclass { }
}
