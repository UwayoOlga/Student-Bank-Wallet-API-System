import React, { createContext, useContext, useState, ReactNode } from 'react';
import { StudentProfile } from '../types/api';

interface AuthContextType {
  student: StudentProfile | null;
  login: (student: StudentProfile) => void;
  logout: () => void;
  updateProfile: (profile: StudentProfile) => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [student, setStudent] = useState<StudentProfile | null>(null);

  const login = (studentData: StudentProfile) => {
    setStudent(studentData);
    localStorage.setItem('student', JSON.stringify(studentData));
  };

  const logout = () => {
    setStudent(null);
    localStorage.removeItem('student');
  };

  const updateProfile = (profileData: StudentProfile) => {
    if (student) {
      const updatedStudent = {
        ...profileData,
        token: profileData.token || student.token // Preserve existing token if new data doesn't have it
      };
      setStudent(updatedStudent);
      localStorage.setItem('student', JSON.stringify(updatedStudent));
    }
  };

  // Check if user is logged in on app start
  React.useEffect(() => {
    const savedStudent = localStorage.getItem('student');
    if (savedStudent) {
      setStudent(JSON.parse(savedStudent));
    }
  }, []);

  const value = {
    student,
    login,
    logout,
    updateProfile,
    isAuthenticated: !!student,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};